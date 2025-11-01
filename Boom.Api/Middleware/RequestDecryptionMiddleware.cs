using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Boom.Business.Services;
using Boom.Infrastructure.Data;
using Boom.Infrastructure.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Boom.Api.Middleware;

public class RequestDecryptionMiddleware
{
    public const string RequestEncryptedItemKey = "RequestEncrypted";

    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RequestDecryptionMiddleware> _logger;

    public RequestDecryptionMiddleware(
        RequestDelegate next,
        IServiceScopeFactory scopeFactory,
        ILogger<RequestDecryptionMiddleware> logger)
    {
        _next = next;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Items[RequestEncryptedItemKey] = false;

        if (!HttpMethods.IsPost(context.Request.Method) &&
            !HttpMethods.IsPut(context.Request.Method) &&
            !HttpMethods.IsPatch(context.Request.Method))
        {
            await _next(context);
            return;
        }

        try
        {
            var envelope = await ExtractEnvelopeAsync(context.Request);
            if (envelope == null ||
                string.IsNullOrEmpty(envelope.EncryptedPayload) ||
                string.IsNullOrEmpty(envelope.InitializationVector) ||
                !Guid.TryParse(envelope.UserUuid, out var playerUuid))
            {
                await _next(context);
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetService(typeof(IRepository)) as IRepository;
            var encryptionService = scope.ServiceProvider.GetService(typeof(IEncryptionService)) as IEncryptionService;

            if (repository == null || encryptionService == null)
            {
                await _next(context);
                return;
            }

            var player = repository.GetAll<Player>().FirstOrDefault(p => p.Uuid == playerUuid);
            if (player?.SecretKey is null)
            {
                await _next(context);
                return;
            }

            var decryptionKey = encryptionService.GetDecryptionKey(player.SecretKey);
            var decryptedPayload = encryptionService.Decrypt(
                envelope.EncryptedPayload,
                decryptionKey,
                envelope.InitializationVector);

            if (string.IsNullOrWhiteSpace(decryptedPayload))
            {
                await _next(context);
                return;
            }

            var parsedForm = BuildFormCollection(decryptedPayload, envelope.ContentType);
            if (parsedForm == null)
            {
                await _next(context);
                return;
            }

            ReplaceRequestBody(context.Request, decryptedPayload, envelope.ContentType);
            context.Features.Set<IFormFeature>(new DecryptedFormFeature(context.Request, parsedForm));
            context.Items[RequestEncryptedItemKey] = true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to decrypt incoming request.");
        }

        await _next(context);
    }

    private static void ReplaceRequestBody(HttpRequest request, string payload, string? contentType)
    {
        var bytes = Encoding.UTF8.GetBytes(payload);
        var newBody = new MemoryStream(bytes);
        request.Body = newBody;
        request.ContentLength = bytes.Length;

        if (!string.IsNullOrWhiteSpace(contentType))
        {
            request.ContentType = contentType;
            request.Headers["Content-Type"] = contentType;
        }

        newBody.Seek(0, SeekOrigin.Begin);
    }

    private static FormCollection? BuildFormCollection(string payload, string? contentType)
    {
        contentType ??= "application/x-www-form-urlencoded";

        Dictionary<string, StringValues> values;
        if (contentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase))
        {
            values = ParseMultipartPayload(payload, contentType);
        }
        else
        {
            var parsed = QueryHelpers.ParseQuery(payload);
            values = parsed.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        if (values.Count == 0)
        {
            return null;
        }

        return new FormCollection(values);
    }

    private static Dictionary<string, StringValues> ParseMultipartPayload(string payload, string contentType)
    {
        var result = new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase);
        var boundary = GetBoundary(contentType);
        if (string.IsNullOrEmpty(boundary))
        {
            return result;
        }

        var delimiter = "--" + boundary;
        var segments = payload.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

        foreach (var rawSegment in segments)
        {
            var segment = rawSegment.Trim('\r', '\n');
            if (segment.Equals("--", StringComparison.Ordinal))
            {
                continue;
            }

            var headerEnd = segment.IndexOf("\r\n\r\n", StringComparison.Ordinal);
            if (headerEnd < 0)
            {
                continue;
            }

            var headers = segment[..headerEnd];
            var body = segment[(headerEnd + 4)..].TrimEnd('\r', '\n');

            var dispositionMatch = Regex.Match(headers, "name=\"(?<name>[^\"]+)\"", RegexOptions.IgnoreCase);
            if (!dispositionMatch.Success)
            {
                continue;
            }

            var name = dispositionMatch.Groups["name"].Value;
            if (string.IsNullOrEmpty(name))
            {
                continue;
            }

            if (result.TryGetValue(name, out var existing))
            {
                var combined = existing.ToArray().ToList();
                combined.Add(body);
                result[name] = new StringValues(combined.ToArray());
            }
            else
            {
                result[name] = new StringValues(body);
            }
        }

        return result;
    }

    private static string? GetBoundary(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return null;
        }

        var segments = contentType.Split(';', StringSplitOptions.RemoveEmptyEntries);
        foreach (var segment in segments)
        {
            var trimmed = segment.Trim();
            if (trimmed.StartsWith("boundary=", StringComparison.OrdinalIgnoreCase))
            {
                var boundary = trimmed[9..];
                if (boundary.Length >= 2 && boundary.StartsWith('"') && boundary.EndsWith('"'))
                {
                    boundary = boundary[1..^1];
                }

                return boundary;
            }
        }

        return null;
    }

    private static async Task<EncryptedRequestEnvelope?> ExtractEnvelopeAsync(HttpRequest request)
    {
        string? encryptedPayload = GetFirstValue(request.Query, "_p");
        string? iv = GetFirstValue(request.Query, "_s");
        string? uuid = GetFirstValue(request.Query, "_u");
        string? contentType = GetFirstValue(request.Query, "_ct");

        if (NeedsBodyExtraction(encryptedPayload, iv, uuid, contentType))
        {
            request.EnableBuffering();
            request.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var rawBody = await reader.ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);

            if (!string.IsNullOrEmpty(rawBody))
            {
                var bodyValues = ParseRawBody(rawBody, request.ContentType);
                encryptedPayload ??= GetFirstValue(bodyValues, "_p");
                iv ??= GetFirstValue(bodyValues, "_s");
                uuid ??= GetFirstValue(bodyValues, "_u");
                contentType ??= GetFirstValue(bodyValues, "_ct");
            }
        }

        if (string.IsNullOrEmpty(encryptedPayload) || string.IsNullOrEmpty(iv) || string.IsNullOrEmpty(uuid))
        {
            return null;
        }

        return new EncryptedRequestEnvelope
        {
            EncryptedPayload = encryptedPayload,
            InitializationVector = iv,
            UserUuid = uuid,
            ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/x-www-form-urlencoded" : contentType
        };
    }

    private static bool NeedsBodyExtraction(params string?[] values)
    {
        return values.Any(string.IsNullOrEmpty);
    }

    private static Dictionary<string, StringValues> ParseRawBody(string rawBody, string? contentType)
    {
        if (!string.IsNullOrWhiteSpace(contentType) &&
            contentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase))
        {
            return ParseMultipartPayload(rawBody, contentType);
        }

        return QueryHelpers.ParseQuery(rawBody).ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    private static string? GetFirstValue(IQueryCollection queryCollection, string key)
    {
        return queryCollection.TryGetValue(key, out var value) ? value.FirstOrDefault() : null;
    }

    private static string? GetFirstValue(Dictionary<string, StringValues> values, string key)
    {
        return values.TryGetValue(key, out var value) ? value.FirstOrDefault() : null;
    }

    private sealed class DecryptedFormFeature : IFormFeature
    {
        private readonly string? _contentType;
        private IFormCollection? _form;

        public DecryptedFormFeature(HttpRequest request, IFormCollection form)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            _form = form ?? throw new ArgumentNullException(nameof(form));
            _contentType = request.ContentType;
        }

        public bool HasFormContentType => IsFormContentType(_contentType);

        public IFormCollection Form
        {
            get => _form ??= new FormCollection(new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase));
            set => _form = value;
        }

        public Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(Form);

        private static bool IsFormContentType(string? contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                return false;
            }

            if (!MediaTypeHeaderValue.TryParse(contentType, out var parsed))
            {
                return false;
            }

            var mediaType = parsed.MediaType;
            if (string.IsNullOrEmpty(mediaType))
            {
                return false;
            }

            return mediaType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase)
                || mediaType.Equals("multipart/form-data", StringComparison.OrdinalIgnoreCase)
                || mediaType.Equals("text/plain", StringComparison.OrdinalIgnoreCase);
        }
    }

    private sealed class EncryptedRequestEnvelope
    {
        public string? EncryptedPayload { get; init; }
        public string? InitializationVector { get; init; }
        public string? UserUuid { get; init; }
        public string? ContentType { get; init; }
    }
}
