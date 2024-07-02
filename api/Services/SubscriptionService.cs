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
        if (!await pttClient.IsBoardExist(board))
        {
            throw new CommandException($"{board} Board Not Exist");
        }

        await subscriptionRepository.Add(userId, board, keyword, null);
        if (!await subscribedBoardRepository.IsExist(board))
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

    public async Task SubscribeAuthor(long userId, string board, string author)
    {
        if (!await pttClient.IsBoardExist(board))
        {
            throw new CommandException($"{board} Board Not Exist");
        }

        await subscriptionRepository.Add(userId, board, null, author);
        if (!await subscribedBoardRepository.IsExist(board))
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
        await subscriptionRepository.Delete(userId, board, keyword, null);
        if (!await subscriptionRepository.IsBoardSubscribed(board))
        {
            await subscribedBoardRepository.Delete(board);
        }
    }

    public async Task UnsubscribeAuthor(long userId, string board, string author)
    {
        await subscriptionRepository.Delete(userId, board, null, author);
        if (!await subscriptionRepository.IsBoardSubscribed(board))
        {
            await subscribedBoardRepository.Delete(board);
        }
    }
}

public class CommandException : Exception
{
    public CommandException(string? message) : base(message)
    {
    }

    public CommandException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}