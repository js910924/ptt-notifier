using HtmlAgilityPack;
using infrastructure.Models;
using Article = domain.Models.Article;

namespace infrastructure;

public class PttClient(HttpClient httpClient) : IPttClient
{
    public const string PttUrl = "https://www.ptt.cc";
    private static string GetPttBoardIndexUrl(string board) => $"{PttUrl}/bbs/{board}/index.html";

    public async Task<List<Article>> SearchPttArticlesAsync(string board, int days)
    {
        var startDate = DateTime.Today.AddDays(-days).Date;
        var url = GetPttBoardIndexUrl(board);

        var articles = new List<Article>();

        while (true)
        {
            var doc = await GetPttPageHtmlDocument(board, url);
            var articlesInPage = doc.GetArticlesInPage();

            articles = articlesInPage.Where(article => article.Date >= startDate).Concat(articles).ToList();

            if (articlesInPage.Count != 0 && articlesInPage.TrueForAll(a => a.Date >= startDate))
            {
                url = doc.GetPreviousPageUrl();
            }
            else
            {
                break;
            }
        }

        return articles;
    }

    public async Task<Article> GetLatestArticle(string board)
    {
        var url = GetPttBoardIndexUrl(board);

        var doc = await GetPttPageHtmlDocument(board, url);
        return doc.GetArticlesInPage().Last();
    }

    private async Task<PttPageHtmlDocument> GetPttPageHtmlDocument(string board, string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
        request.Headers.Add("Cookie", "over18=1;");

        var response = await httpClient.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();
        var doc = new HtmlDocument();
        doc.LoadHtml(responseContent);

        return new PttPageHtmlDocument(doc, board);
    }
}