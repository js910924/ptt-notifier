namespace api.Requests;

public class SubscribeRequest(int userId, string board, string keyword)
{
    public int UserId { get; private set; } = userId;
    public string Board { get; private set; } = board;
    public string Keyword { get; private set; } = keyword;
}