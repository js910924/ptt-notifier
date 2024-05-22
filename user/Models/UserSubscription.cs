namespace user.Models;

public class UserSubscription
{
    private sealed class UserIdBoardKeywordEqualityComparer : IEqualityComparer<UserSubscription>
    {
        public bool Equals(UserSubscription x, UserSubscription y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.UserId == y.UserId && x.Board == y.Board && x.Keyword == y.Keyword;
        }

        public int GetHashCode(UserSubscription obj)
        {
            return HashCode.Combine(obj.UserId, obj.Board, obj.Keyword);
        }
    }

    public static IEqualityComparer<UserSubscription> UserIdBoardKeywordComparer { get; } = new UserIdBoardKeywordEqualityComparer();

    public int UserId { get; set; }
    public string Board { get; set; }
    public string Keyword { get; set; }
}
