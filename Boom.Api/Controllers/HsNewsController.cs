using System.Text;
using Boom.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Boom.Api.Controllers;

[ApiController]
public class HsNewsController : ControllerBase
{
    private readonly IArticleService _articleService;
    private readonly IPlistSerializationService _plistService;

    public HsNewsController(IArticleService articleService, IPlistSerializationService plistService)
    {
        _articleService = articleService;
        _plistService = plistService;
    }

    // Plain unencrypted GET: the news feed is not part of the encrypted request/response pipeline.
    [HttpGet("hsnews/feed/Boom/{locale}/{type?}/{timestamp?}")]
    public async Task<IActionResult> Show(string locale, string? type, long? timestamp)
    {
        var feed = await _articleService.GetFeed(timestamp);
        var plistXml = _plistService.ToPlistString(feed);

        return File(Encoding.UTF8.GetBytes(plistXml), "application/x-plist");
    }
}
