using domain.Models;

namespace infrastructure;

public interface ISubscriptionRepository
{
    Task<List<Subscription>> GetAll();
    Task Add(long userId, string board, string keyword);
    Task Delete(int userId, string board, string keyword);
    Task<List<Subscription>> Get(string board);
}