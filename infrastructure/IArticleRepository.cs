namespace infrastructure;

public interface IArticleRepository
{
    Task<List<domain.Models.Article>> GetAll();
    Task Add(List<domain.Models.Article> articles);
    Task Delete(List<domain.Models.Article> articles);
}