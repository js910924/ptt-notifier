using domain.Models;
using infrastructure;

namespace api.Services;

public class SubscriptionService(
    ISubscriptionRepository subscriptionRepository,
    ISubscribedBoardRepository subscribedBoardRepository,
    IPttClient pttClient) : ISubscriptionService
{
    public async Task Subscribe(long userId, string board, string keyword)
    {
        await subscriptionRepository.Add(userId, board, keyword);
        var subscribedBoards = await subscribedBoardRepository.GetAll();
        if (!subscribedBoards.Exists(subscribedBoard => subscribedBoard.Board == board))
        {
            var latestArticle = await pttClient.GetLatestArticle(board);
            var subscribedBoard = new SubscribedBoard
            {
                Board = board,
                LastLatestArticleTitle = latestArticle.Title
            };
            await subscribedBoardRepository.Add(subscribedBoard);
        }
    }

    public async Task Unsubscribe(long userId, string board, string keyword)
    {
        await subscriptionRepository.Delete(userId, board, keyword);
        var subscriptions = await subscriptionRepository.GetAll();
        if (subscriptions.All(subscription => subscription.Board != board))
        {
            await subscribedBoardRepository.Delete(board);
        }
    }
}