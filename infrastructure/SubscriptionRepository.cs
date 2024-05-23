using infrastructure.Models;
using Supabase;

namespace infrastructure;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly Client _client;

    public SubscriptionRepository(Client client)
    {
        _client = client;
    }

    public async Task<List<domain.Models.Subscription>> GetAll()
    {
        var result = await _client.From<Subscription>().Get();

        return result.Models.Select(model => new domain.Models.Subscription
        {
            UserId = model.UserId,
            Board = model.Board,
            Keyword = model.Keyword,
        }).ToList();
    }

    public async Task Add(long userId, string board, string keyword)
    {
        _ = await _client.From<Subscription>()
            .Upsert(new Subscription
            {
                UserId = userId,
                Board = board,
                Keyword = keyword
            });
    }

    public async Task Delete(int userId, string board, string keyword)
    {
        await _client.From<Subscription>()
            .Delete(new Subscription
            {
                UserId = userId,
                Board = board,
                Keyword = keyword
            });
    }
}