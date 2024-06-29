using infrastructure.Models;
using Supabase;

namespace infrastructure
{
    public class SubscribedBoardRepository(Client client) : ISubscribedBoardRepository
    {
        public async Task<List<domain.Models.SubscribedBoard>> GetAll()
        {
            return (await client.From<SubscribedBoard>().Get()).Models
                .Select(board => new domain.Models.SubscribedBoard
                {
                    Board = board.Board,
                    LastLatestArticleTitle = board.LastLatestArticleTitle,
                }).ToList();
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