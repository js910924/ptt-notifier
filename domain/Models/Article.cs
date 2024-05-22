namespace domain.Models;

public class Article
{
    private sealed class ArticleEqualityComparer : IEqualityComparer<Article>
    {
        public bool Equals(Article x, Article y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Title == y.Title && x.Link == y.Link && x.Date.Equals(y.Date) && x.Author == y.Author && x.Board == y.Board;
        }

        public int GetHashCode(Article obj)
        {
            return HashCode.Combine(obj.Title, obj.Link, obj.Date, obj.Author, obj.Board);
        }
    }

    public static IEqualityComparer<Article> ArticleComparer { get; } = new ArticleEqualityComparer();

    public string Title { get; set; }
    public string Link { get; set; }
    public DateTime Date { get; set; }
    public string Author { get; set; }
    public string Board { get; set; }
}