namespace api.Services;

public interface ISubscriptionService
{
    Task Subscribe(long userId, string board, string keyword);
    Task Unsubscribe(long userId, string board, string keyword);
}