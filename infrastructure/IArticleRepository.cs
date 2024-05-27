namespace infrastructure;

public interface IArticleRepository
{
    Task<List<domain.Models.Article>> GetAll();
    Task Add(List<domain.Models.Article> articles);
    Task Delete(long id);
    Task Delete(string board);
}