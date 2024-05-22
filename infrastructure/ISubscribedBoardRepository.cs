using domain.Models;

namespace infrastructure;

public interface ISubscribedBoardRepository
{
    List<SubscribedBoard> GetAll();
    void Add(string board);
    void Remove(string board);
    void UpdateLatestArticle(string board, string articleTitle);
}