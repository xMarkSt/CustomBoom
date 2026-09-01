using Boom.Business.Services;
using Boom.Common.DTOs.Response;
using Boom.Infrastructure.Data;
using Boom.Infrastructure.Data.Entities;
using FluentAssertions;
using MockQueryable;
using Moq;

namespace Boom.UnitTests.Services
{
    [TestFixture]
    public class ArticleServiceTests
    {
        private Mock<IRepository> _mockRepository;
        private ArticleService _articleService;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IRepository>();
            _articleService = new ArticleService(_mockRepository.Object);
        }

        [Test]
        public async Task GetFeed_NoArticles_ReturnsEmptyListAndZeroCounts()
        {
            _mockRepository.Setup(r => r.GetAll<Article>()).Returns(new List<Article>().AsQueryable().BuildMock());

            var result = await _articleService.GetFeed(null);

            result.Articles.Should().BeOfType<List<ArticleDto>>().Which.Should().BeEmpty();
            result.Count.Should().Be(0);
            result.New.Should().Be(0);
        }

        [Test]
        public async Task GetFeed_ArticlesExist_ReturnsDictionaryKeyedByTimestampAndId()
        {
            var createdAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var article = new Article
            {
                Id = 42,
                Title = "Title",
                Message = "Message",
                Link = "https://example.com",
                LinkTitle = "Read more",
                Popup = true,
                CreatedAt = createdAt,
            };
            _mockRepository.Setup(r => r.GetAll<Article>()).Returns(new List<Article> { article }.AsQueryable().BuildMock());

            var result = await _articleService.GetFeed(null);

            var expectedTimestamp = new DateTimeOffset(createdAt).ToUnixTimeSeconds();
            var dict = result.Articles.Should().BeOfType<Dictionary<string, ArticleDto>>().Subject;
            dict.Should().ContainKey($"{expectedTimestamp}.42");
            var dto = dict[$"{expectedTimestamp}.42"];
            dto.Id.Should().Be(42);
            dto.Timestamp.Should().Be(expectedTimestamp);
            dto.Title.Should().Be("Title");
            dto.LinkTitle.Should().Be("Read more");
            dto.Popup.Should().Be(1);
            result.Count.Should().Be(1);
            result.New.Should().Be(1);
        }

        [Test]
        public async Task GetFeed_WithTimestamp_OnlyReturnsArticlesCreatedAfter()
        {
            var oldArticle = new Article { Id = 1, CreatedAt = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
            var newArticle = new Article { Id = 2, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
            _mockRepository.Setup(r => r.GetAll<Article>())
                .Returns(new List<Article> { oldArticle, newArticle }.AsQueryable().BuildMock());

            var afterTimestamp = new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)).ToUnixTimeSeconds();

            var result = await _articleService.GetFeed(afterTimestamp);

            result.Count.Should().Be(1);
            var dict = result.Articles.Should().BeOfType<Dictionary<string, ArticleDto>>().Subject;
            dict.Values.Single().Id.Should().Be(2);
        }
    }
}
