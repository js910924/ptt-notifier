using domain.Models;

namespace infrastructure;

public class SubscriptionRepository
{
    private readonly HashSet<Subscription> _subscriptions = new(Subscription.UserIdBoardKeywordComparer);

    public List<Subscription> GetAll()
    {
        return _subscriptions.ToList();
    }

    public void Add(int userId, string board, string keyword)
    {
        _subscriptions.Add(new Subscription
        {
            UserId = userId,
            Board = board,
            Keyword = keyword
        });
    }

    public void Remove(int userId, string board, string keyword)
    {
        _subscriptions.Remove(new Subscription
        {
            UserId = userId,
            Board = board,
            Keyword = keyword
        });
    }
}