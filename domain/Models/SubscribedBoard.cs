namespace domain.Models;

public class SubscribedBoard
{
    private sealed class SubscribedBoardEqualityComparer : IEqualityComparer<SubscribedBoard>
    {
        public bool Equals(SubscribedBoard x, SubscribedBoard y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Board == y.Board;
        }

        public int GetHashCode(SubscribedBoard obj)
        {
            return obj.Board.GetHashCode();
        }
    }

    public static IEqualityComparer<SubscribedBoard> SubscribedBoardComparer { get; } = new SubscribedBoardEqualityComparer();

    public string Board { get; set; }
    public string? LastLatestArticleTitle { get; set; }
}