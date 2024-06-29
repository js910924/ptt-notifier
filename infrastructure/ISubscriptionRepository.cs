using domain.Models;

namespace infrastructure;

public interface ISubscriptionRepository
{
    Task<List<Subscription>> GetAll();
    Task Add(long userId, string board, string keyword, string author);
    Task Delete(long userId, string board, string keyword, string author);
    Task<List<Subscription>> Get(string board);
    Task<List<Subscription>> Get(long userId);
    Task<bool> IsBoardSubscribed(string board);
}