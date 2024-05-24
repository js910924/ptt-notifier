using HtmlAgilityPack;
using infrastructure.Models;
using Article = domain.Models.Article;

namespace infrastructure;

public class PttClient : IPttClient
{
    public const string PttUrl = "https://www.ptt.cc";
    private readonly HttpClient _httpClient;

    public PttClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Article>> SearchPttArticlesAsync(string board, int days)
    {
        var startDate = DateTime.Today.AddDays(-days).Date;
        var url = $"{PttUrl}/bbs/{board}/index.html";

        var articles = new List<Article>();

        while (true)
        {
            var (previousPageUrl, articlesInPage) = await GetArticlesInPage(board, url);

            articles = articlesInPage.Where(article => article.Date >= startDate).Concat(articles).ToList();

            if (articlesInPage.Count != 0 && articlesInPage.TrueForAll(a => a.Date >= startDate))
            {
                url = previousPageUrl;
            }
            else
            {
                break;
            }
        }

        return articles;
    }

    private async Task<(string previousPageUrl, List<Article> articlesInPage)> GetArticlesInPage(string board, string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
        request.Headers.Add("Cookie", "over18=1;");

        var response = await _httpClient.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();
        var doc = new HtmlDocument();
        doc.LoadHtml(responseContent);

        var articlesInPage = new PttPageHtmlDocument(doc, board).GetArticlesInPage();
        var nextPageButton = doc.DocumentNode.SelectSingleNode("//div[@class='btn-group btn-group-paging']/a[contains(text(), '上頁')]");
        var previousPageUrl = PttUrl + nextPageButton.GetAttributeValue("href", "");
        return (previousPageUrl, articlesInPage);
    }
}