using infrastructure.Models;
using Supabase;

namespace infrastructure;

public class SubscriptionRepository(Client client) : ISubscriptionRepository
{
    public async Task<List<domain.Models.Subscription>> GetAll()
    {
        var result = await client.From<Subscription>().Get();

        return result.Models.Select(subscription => subscription.ToDomainModel()).ToList();
    }

    public async Task Add(long userId, string board, string keyword, string author)
    {
        // TODO: search how to prevent duplicate insert in Supabase
        if ((await GetAll()).Any(subscription =>
                subscription.UserId == userId
                && subscription.Board == board
                && subscription.Keyword == keyword
                && subscription.Author == author))
        {
            return;
        }

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
        var deletedSubscription = (await GetAll())
            .SingleOrDefault(subscription =>
                subscription.UserId == userId
                && subscription.Board.Equals(board.ToLower())
                && subscription.Keyword == keyword
                && subscription.Author == author);
        if (deletedSubscription is null)
        {
            return;
        }

        await client.From<Subscription>()
            .Where(subscription => subscription.Id == deletedSubscription.Id)
            .Delete();
    }

    public async Task<List<domain.Models.Subscription>> Get(string board)
    {
        return (await client.From<Subscription>()
                .Where(subscription => subscription.Board == board)
                .Get()).Models
            .Select(subscription => subscription.ToDomainModel())
            .ToList();
    }

    public async Task<List<domain.Models.Subscription>> Get(long userId)
    {
        return (await client.From<Subscription>()
                .Where(subscription => subscription.UserId == userId)
                .Get()).Models
            .Select(subscription => subscription.ToDomainModel())
            .ToList();
    }

    public async Task<bool> IsBoardSubscribed(string board)
    {
        var subscriptions = await GetAll();
        return subscriptions.Exists(subscription => subscription.Board == board);
    }
}