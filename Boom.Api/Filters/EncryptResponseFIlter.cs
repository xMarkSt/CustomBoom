using Boom.Api.Middleware;
using Boom.Business.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Boom.Common;

namespace Boom.Api.Filters;

public class EncryptResponseFilter : IActionFilter
{
    private readonly IEncryptionService _encryptionService;
    private readonly IPlistSerializationService _plistService;

    public EncryptResponseFilter(
        IEncryptionService encryptionService,
        IPlistSerializationService plistService)
    {
        _encryptionService = encryptionService;
        _plistService = plistService;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // no-op
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Only continue if the action returned an IPlistSerializable payload
        if (context.Result is not ObjectResult { Value: IPlistSerializable dto })
            return;

        var httpContext = context.HttpContext;
        var items = httpContext.Items;

        var requestEncrypted =
            items.TryGetValue(RequestDecryptionMiddleware.RequestEncryptedItemKey, out var encryptedObj)
            && encryptedObj is true;

        // Serialize DTO → plist XML
        var plistXml = _plistService.ToPlistString(dto);

        string output;
        string contentType;

        if (requestEncrypted &&
            items.TryGetValue(RequestDecryptionMiddleware.EncryptionKeyItemKey, out var keyObj) &&
            keyObj is string encryptionKey &&
            !string.IsNullOrWhiteSpace(encryptionKey))
        {
            output = _encryptionService.Encrypt(plistXml, encryptionKey);
            contentType = "application/ex-plist";
        }
        else
        {
            output = plistXml;
            contentType = "application/x-plist";
        }

        context.Result = new FileContentResult(
            Encoding.UTF8.GetBytes(output),
            contentType
        );
    }
}