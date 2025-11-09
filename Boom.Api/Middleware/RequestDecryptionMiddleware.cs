using System.Text;
using Boom.Business.Services;
using Boom.Infrastructure.Data;
using Boom.Infrastructure.Data.Entities;
using Microsoft.AspNetCore.Http.Features;

namespace Boom.Api.Middleware;

public class RequestDecryptionMiddleware
{
    public const string RequestEncryptedItemKey = "RequestEncrypted";
    public const string EncryptionKeyItemKey = "EncryptionKey";

    private readonly RequestDelegate _next;
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger<RequestDecryptionMiddleware> _logger;

    public RequestDecryptionMiddleware(
        RequestDelegate next,
        ILogger<RequestDecryptionMiddleware> logger,
        IEncryptionService encryptionService)
    {
        _next = next;
        _logger = logger;
        _encryptionService = encryptionService;
    }
    
    private sealed record EncryptedRequestEnvelope(
        string? EncryptedPayload,
        string? InitializationVector,
        string? UserUuid,
        string? ContentType
    );

    public async Task InvokeAsync(HttpContext context, IRepository repository)
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

            var player = repository.GetAll<Player>().FirstOrDefault(p => p.Uuid == playerUuid);
            if (player?.SecretKey is null)
            {
                await _next(context);
                return;
            }

            var decryptionKey = _encryptionService.GetDecryptionKey(player.SecretKey);
            var decryptedPayload = _encryptionService.Decrypt(
                envelope.EncryptedPayload,
                decryptionKey,
                envelope.InitializationVector);

            if (string.IsNullOrWhiteSpace(decryptedPayload))
            {
                await _next(context);
                return;
            }

            ReplaceRequestBody(context.Request, decryptedPayload, envelope.ContentType);
            context.Features.Set<IFormFeature>(null); // force reparse of form
            context.Items[RequestEncryptedItemKey] = true;
            context.Items[EncryptionKeyItemKey] = decryptionKey;
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
        request.ContentLength = null;

        if (!string.IsNullOrWhiteSpace(contentType))
        {
            request.ContentType = contentType;
            request.Headers["Content-Type"] = contentType;
        }

        newBody.Seek(0, SeekOrigin.Begin);
    }

    private static async Task<EncryptedRequestEnvelope?> ExtractEnvelopeAsync(HttpRequest request)
    {
        if (!request.HasFormContentType)
            return null;
        
        var form = await request.ReadFormAsync();
        var encryptedPayload = form["_p"].FirstOrDefault();
        var iv = form["_s"].FirstOrDefault();
        var uuid = form["_u"].FirstOrDefault();
        var contentType = form["_ct"].FirstOrDefault();

        if (string.IsNullOrEmpty(encryptedPayload) || string.IsNullOrEmpty(iv) || string.IsNullOrEmpty(uuid))
        {
            return null;
        }

        return new EncryptedRequestEnvelope(
            encryptedPayload,
            iv,
            uuid,
            string.IsNullOrWhiteSpace(contentType)
                ? "application/x-www-form-urlencoded"
                : contentType
        );
    }
}
