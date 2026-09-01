using Boom.Common.DTOs.Response;

namespace Boom.Business.Services;

public interface IArticleService
{
    Task<NewsFeedDto> GetFeed(long? afterTimestamp);
}
