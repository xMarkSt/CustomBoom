using Microsoft.AspNetCore.Http;

namespace Boom.Common.Extensions;

// Source - https://stackoverflow.com/a/59359240
// Posted by Rookian, modified by community. See post 'Timeline' for change history
// Retrieved 2026-04-04, License - CC BY-SA 4.0

public static class FormFileExtensions
{
    public static async Task<byte[]> GetBytes(this IFormFile formFile)
    {
        await using var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}
