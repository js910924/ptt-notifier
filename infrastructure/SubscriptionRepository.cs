using infrastructure.Models;
using Supabase;

namespace infrastructure;

public class SubscriptionRepository(Client client) : ISubscriptionRepository
{
    public async Task<List<domain.Models.Subscription>> GetAll()
    {
        var result = await client.From<Subscription>().Get();

        return result.Models.Select(model => new domain.Models.Subscription
        {
            UserId = model.UserId,
            Board = model.Board,
            Keyword = model.Keyword,
            Author = model.Author,
        }).ToList();
    }

    public async Task Add(long userId, string board, string keyword, string author)
    {
        _ = await client.From<Subscription>()
            .Insert(new Subscription
            {
                UserId = userId,
                Board = board.ToLower(),
                Keyword = keyword,
                Author = author,
                CreatedAt = DateTime.Now,
            });
    }

    public async Task Delete(long userId, string board, string keyword, string author)
    {
        await client.From<Subscription>()
            .Delete(new Subscription
            {
                UserId = userId,
                Board = board.ToLower(),
                Keyword = keyword,
                Author = author,
            });
    }

    public async Task<List<domain.Models.Subscription>> Get(string board)
    {
        return (await client.From<Subscription>()
                .Where(subscription => subscription.Board == board)
                .Get()).Models
            .Select(subscription => new domain.Models.Subscription
            {
                UserId = subscription.UserId,
                Board = subscription.Board,
                Keyword = subscription.Keyword,
                Author = subscription.Author,
            })
            .ToList();
    }

    public async Task<List<domain.Models.Subscription>> Get(long userId)
    {
        return (await client.From<Subscription>()
                .Where(subscription => subscription.UserId == userId)
                .Get()).Models
            .Select(subscription => new domain.Models.Subscription
            {
                UserId = subscription.UserId,
                Board = subscription.Board,
                Keyword = subscription.Keyword,
                Author = subscription.Author,
            })
            .ToList();
    }
}