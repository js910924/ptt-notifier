using domain.Models;
using Supabase;

namespace infrastructure;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly Client _client;

    public SubscriptionRepository(Client client)
    {
        _client = client;
    }

    public async Task<List<Subscription>> GetAll()
    {
        var result = await _client.From<infrastructure.Models.Subscription>().Get();

        return result.Models.Select(model => new Subscription
        {
            UserId = model.UserId,
            Board = model.Board,
            Keyword = model.Keyword,
        }).ToList();
    }

    public async Task Add(int userId, string board, string keyword)
    {
        _ = await _client.From<infrastructure.Models.Subscription>()
            .Upsert(new infrastructure.Models.Subscription
            {
                UserId = userId,
                Board = board,
                Keyword = keyword
            });
    }

    public async Task Delete(int userId, string board, string keyword)
    {
        await _client.From<infrastructure.Models.Subscription>()
            .Delete(new infrastructure.Models.Subscription
            {
                UserId = userId,
                Board = board,
                Keyword = keyword
            });
    }
}