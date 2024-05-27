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
                Board = board.ToLower(),
                Keyword = keyword
            });
    }

    public async Task Delete(long userId, string board, string keyword)
    {
        await _client.From<Subscription>()
            .Delete(new Subscription
            {
                UserId = userId,
                Board = board.ToLower(),
                Keyword = keyword
            });
    }

    public async Task<List<domain.Models.Subscription>> Get(string board)
    {
        return (await _client.From<Subscription>()
                .Where(subscription => subscription.Board == board)
                .Get()).Models
            .Select(subscription => new domain.Models.Subscription
            {
                UserId = subscription.UserId,
                Board = subscription.Board,
                Keyword = subscription.Keyword,
            })
            .ToList();
    }

    public async Task<List<domain.Models.Subscription>> Get(long userId)
    {
        return (await _client.From<Subscription>()
                .Where(subscription => subscription.UserId == userId)
                .Get()).Models
            .Select(subscription => new domain.Models.Subscription
            {
                UserId = subscription.UserId,
                Board = subscription.Board,
                Keyword = subscription.Keyword,
            })
            .ToList();
    }
}