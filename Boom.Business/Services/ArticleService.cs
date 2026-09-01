using Boom.Common.DTOs.Response;
using Boom.Infrastructure.Data;
using Boom.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Boom.Business.Services;

public class ArticleService : IArticleService
{
    private readonly IRepository _repository;

    public ArticleService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<NewsFeedDto> GetFeed(long? afterTimestamp)
    {
        var query = _repository.GetAll<Article>();

        if (afterTimestamp.HasValue)
        {
            var afterDate = DateTimeOffset.FromUnixTimeSeconds(afterTimestamp.Value).UtcDateTime;
            query = query.Where(a => a.CreatedAt > afterDate);
        }

        var articles = await query.ToListAsync();

        var feed = new NewsFeedDto
        {
            Count = articles.Count,
            New = articles.Count,
        };

        if (articles.Count == 0)
        {
            feed.Articles = new List<ArticleDto>();
        }
        else
        {
            var articlesByKey = new Dictionary<string, ArticleDto>();
            foreach (var article in articles)
            {
                var createdAt = DateTime.SpecifyKind(article.CreatedAt ?? DateTime.UtcNow, DateTimeKind.Utc);
                var timestamp = new DateTimeOffset(createdAt).ToUnixTimeSeconds();
                articlesByKey[$"{timestamp}.{article.Id}"] = new ArticleDto
                {
                    Id = article.Id,
                    Timestamp = timestamp,
                    Title = article.Title,
                    Message = article.Message,
                    Link = article.Link,
                    LinkTitle = article.LinkTitle,
                    Popup = article.Popup ? 1 : 0,
                };
            }

            feed.Articles = articlesByKey;
        }

        return feed;
    }
}
