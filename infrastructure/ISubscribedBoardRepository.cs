using domain.Models;

namespace infrastructure;

public interface ISubscribedBoardRepository
{
    Task<List<SubscribedBoard>> GetAll();
    Task Add(SubscribedBoard board);
    Task Delete(string board);
    Task UpdateLatestArticle(string board, string articleTitle);
    Task<bool> IsExist(string board);
}