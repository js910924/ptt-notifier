using infrastructure.Models;
using Supabase;

namespace infrastructure
{
    public class SubscribedBoardRepository(Client client) : ISubscribedBoardRepository
    {
        public async Task<List<domain.Models.SubscribedBoard>> GetAll()
        {
            return (await client.From<SubscribedBoard>().Get()).Models
                .Select(board => board.ToDomainModel()).ToList();
        }

        public async Task<bool> IsExist(string board)
        {
            return (await client.From<SubscribedBoard>().Get()).Models
                .Exists(subscribedBoard => subscribedBoard.Board.Equals(board.ToLower()));
        }

        public async Task Add(domain.Models.SubscribedBoard board)
        {
            _ = await client.From<SubscribedBoard>()
                .Upsert(new SubscribedBoard
                {
                    Board = board.Board.ToLower(),
                    LastLatestArticleTitle = board.LastLatestArticleTitle,
                });
        }

        public async Task Delete(string board)
        {
            _ = await client.From<SubscribedBoard>()
                .Delete(new SubscribedBoard
                {
                    Board = board
                });
        }

        public async Task UpdateLatestArticle(string board, string articleTitle)
        {
            _ = await client.From<SubscribedBoard>()
                .Where(x => x.Board == board)
                .Set(x => x.LastLatestArticleTitle, articleTitle)
                .Update();
        }
    }
}