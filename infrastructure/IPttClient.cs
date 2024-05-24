using domain.Models;

namespace infrastructure;

public interface IPttClient
{
    Task<List<Article>> SearchPttArticlesAsync(string board, int days);
    Task<Article> GetLatestArticle(string board);
}