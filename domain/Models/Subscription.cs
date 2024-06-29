namespace domain.Models;

public class Subscription
{
    private sealed class UserIdBoardKeywordEqualityComparer : IEqualityComparer<Subscription>
    {
        public bool Equals(Subscription x, Subscription y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.UserId == y.UserId && x.Board == y.Board && x.Keyword == y.Keyword && x.Author == y.Author;
        }

        public int GetHashCode(Subscription obj)
        {
            return HashCode.Combine(obj.UserId, obj.Board, obj.Keyword, obj.Author);
        }
    }

    public static IEqualityComparer<Subscription> UserIdBoardKeywordComparer { get; } = new UserIdBoardKeywordEqualityComparer();

    public int Id { get; set; }
    public long UserId { get; set; }
    public string Board { get; set; }
    public string? Keyword { get; set; }
    public string? Author { get; set; }
}