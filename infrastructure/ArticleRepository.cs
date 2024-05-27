using infrastructure.Models;
using Supabase;

namespace infrastructure;

public class ArticleRepository : IArticleRepository
{
    private readonly Client _client;

    public ArticleRepository(Client client)
    {
        _client = client;
    }

    public async Task<List<domain.Models.Article>> GetAll()
    {
        var result = await _client.From<Article>().Get();

        return result.Models.Select(model => new domain.Models.Article
        {
            Id = model.Id,
            Board = model.Board,
            Title = model.Title,
            Author = model.Author,
            Link = model.Link,
        }).ToList();
    }

    public async Task Add(List<domain.Models.Article> articles)
    {
        if (articles.Count == 0)
        {
            return;
        }
        var models = articles.Select(article => new Article
        {
            Board = article.Board,
            Title = article.Title,
            Link = article.Link,
            Author = article.Author,
        }).ToList();

        _ = await _client.From<Article>()
            .Insert(models);
    }

    public async Task Delete(long id)
    {
        await _client.From<Article>()
            .Where(x => x.Id == id)
            .Delete();
    }

    public async Task Delete(string board)
    {
        await _client.From<Article>()
            .Where(x => x.Board == board)
            .Delete();
    }
}