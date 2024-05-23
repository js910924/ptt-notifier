using domain.Models;
using fetch_latest_articles_worker.Services;
using infrastructure;
using NSubstitute;

namespace fetch_latest_articles_worker.UnitTests;

public class FetchLatestArticlesServiceTests
{
    private ISubscribedBoardRepository _subscribedBoardRepository;
    private IPttClient _pttClient;
    private FetchLatestArticlesService _sut;

    [SetUp]
    public void Setup()
    {
        _subscribedBoardRepository = Substitute.For<ISubscribedBoardRepository>();
        _pttClient = Substitute.For<IPttClient>();

        _sut = new FetchLatestArticlesService(_subscribedBoardRepository, _pttClient);
    }

    [Test]
    public async Task wont_get_articles_when_latest_article_is_last_latest_article()
    {
        GivenSubscribedBoards(new SubscribedBoard { Board = "Stock", LastLatestArticleTitle = "title 3" });
        GivenArticles(
            new Article { Board = "Stock", Title = "title 1" },
            new Article { Board = "Stock", Title = "title 2" },
            new Article { Board = "Stock", Title = "title 3" }
        );

        var articles = await _sut.Fetch();

        Assert.That(new List<Article>(), Is.EqualTo(articles));
    }

    [Test]
    public async Task wont_get_articles_when_no_articles()
    {
        GivenSubscribedBoards(new SubscribedBoard { Board = "Stock", LastLatestArticleTitle = "title 3" });
        GivenArticles();

        var articles = await _sut.Fetch();

        Assert.That(new List<Article>(), Is.EqualTo(articles));
    }

    [Test]
    public async Task get_latest_articles_according_to_subscribed_board_last_latest_article_title()
    {
        GivenSubscribedBoards(new SubscribedBoard { Board = "Stock", LastLatestArticleTitle = "title 2" });
        GivenArticles(
            new Article { Board = "Stock", Title = "title 1" },
            new Article { Board = "Stock", Title = "title 2" },
            new Article { Board = "Stock", Title = "title 3" }
        );

        var articles = await _sut.Fetch();

        Assert.That(new List<Article>
        {
            new() { Board = "Stock", Title = "title 3" }
        }.SequenceEqual(articles, Article.ArticleComparer), Is.True);
    }

    [Test]
    public async Task get_all_today_articles_when_subscribed_board_last_latest_articles_title_not_in_today_articles()
    {
        GivenSubscribedBoards(new SubscribedBoard { Board = "Stock", LastLatestArticleTitle = "title X" });
        GivenArticles(
            new Article { Board = "Stock", Title = "title 1" },
            new Article { Board = "Stock", Title = "title 2" },
            new Article { Board = "Stock", Title = "title 3" }
        );

        var articles = await _sut.Fetch();

        Assert.That(new List<Article>
        {
            new() { Board = "Stock", Title = "title 1" },
            new() { Board = "Stock", Title = "title 2" },
            new() { Board = "Stock", Title = "title 3" },
        }.SequenceEqual(articles, Article.ArticleComparer), Is.True);
    }

    private void GivenArticles(params Article[] articles)
    {
        _pttClient.SearchPttArticlesAsync(Arg.Any<string>(), Arg.Any<int>()).Returns(articles.ToList());
    }

    private void GivenSubscribedBoards(params SubscribedBoard[] subscribedBoards)
    {
        _subscribedBoardRepository.GetAll().Returns(subscribedBoards.ToList());
    }
}