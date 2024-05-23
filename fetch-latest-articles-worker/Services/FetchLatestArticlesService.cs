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

    public async Task<List<Article>> Fetch(SubscribedBoard board)
    {
        var todayArticles = await _pttClient.SearchPttArticlesAsync(board.Board, 0);
        if (todayArticles.Count > 0 && todayArticles.LastOrDefault()?.Title != board.LastLatestArticleTitle)
        {
            var lastLatestArticleIndex = todayArticles.FindLastIndex(article => article.Title == board.LastLatestArticleTitle);
            if (lastLatestArticleIndex == -1)
            {
                await _subscribedBoardRepository.UpdateLatestArticle(board.Board, todayArticles.Last().Title);
                return todayArticles;
            }

            var latestArticles = todayArticles.GetRange(lastLatestArticleIndex + 1, todayArticles.Count - lastLatestArticleIndex - 1);
            await _subscribedBoardRepository.UpdateLatestArticle(board.Board, latestArticles.Last().Title);
            return latestArticles;
        }

        return [];
    }
}