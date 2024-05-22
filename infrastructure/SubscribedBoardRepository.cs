using domain.Models;

namespace infrastructure;

public class SubscribedBoardRepository : ISubscribedBoardRepository
{
    private readonly HashSet<SubscribedBoard> _subscriptions = new(SubscribedBoard.SubscribedBoardComparer);

    public List<SubscribedBoard> GetAll()
    {
        return _subscriptions.ToList();
    }

    public void Add(string board)
    {
        if (_subscriptions.Any(subscription => subscription.Board.Equals(board, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        _subscriptions.Add(new SubscribedBoard
        {
            Board = board,
        });
    }

    public void Remove(string board)
    {
        _subscriptions.Remove(new SubscribedBoard
        {
            Board = board,
        });
    }

    public void UpdateLatestArticle(string board, string articleTitle)
    {
        var subscribedBoard = _subscriptions.SingleOrDefault(subscribedBoard => subscribedBoard.Board.Equals(board, StringComparison.OrdinalIgnoreCase));
        if (subscribedBoard == null)
        {
            return;
        }

        subscribedBoard.LastLatestArticleTitle = articleTitle;
    }
}