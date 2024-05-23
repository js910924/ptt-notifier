using domain.Models;
using infrastructure;

namespace fetch_latest_articles_worker.Services;

public class FetchLatestArticlesService
{
    private readonly ISubscribedBoardRepository _subscribedBoardRepository;
    private readonly IPttClient _pttClient;

    public FetchLatestArticlesService(ISubscribedBoardRepository subscribedBoardRepository, IPttClient pttClient)
    {
        _subscribedBoardRepository = subscribedBoardRepository;
        _pttClient = pttClient;
    }

    public async Task<List<Article>> Fetch()
    {
        var subscribedBoards = await _subscribedBoardRepository.GetAll();
        var totalArticles = new List<Article>();
        foreach (var subscribedBoard in subscribedBoards)
        {
            Console.WriteLine("LastLatestTitle: " + subscribedBoard.LastLatestArticleTitle);
            var todayArticles = await _pttClient.SearchPttArticlesAsync(subscribedBoard.Board, 0);
            if (todayArticles.Count > 0 && todayArticles.LastOrDefault()?.Title != subscribedBoard.LastLatestArticleTitle)
            {
                var lastLatestArticleIndex = todayArticles.FindLastIndex(article => article.Title == subscribedBoard.LastLatestArticleTitle);
                if (lastLatestArticleIndex == -1)
                {
                    await _subscribedBoardRepository.UpdateLatestArticle(subscribedBoard.Board, todayArticles.Last().Title);
                    totalArticles.AddRange(todayArticles);
                }
                else
                {
                    var latestArticles = todayArticles.GetRange(lastLatestArticleIndex + 1, todayArticles.Count - lastLatestArticleIndex - 1);
                    await _subscribedBoardRepository.UpdateLatestArticle(subscribedBoard.Board, latestArticles.Last().Title);
                    totalArticles.AddRange(latestArticles);
                }
            }
        }

        return totalArticles;
    }
}