using System.Globalization;
using domain.Models;
using HtmlAgilityPack;

namespace infrastructure;

public class PttClient : IPttClient
{
    private readonly HttpClient _httpClient;

    public PttClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Article>> SearchPttArticlesAsync(string board, int days)
    {
        var startDate = DateTime.Today.AddDays(-days).Date;
        var url = $"https://www.ptt.cc/bbs/{board}/index.html";

        var articles = new List<Article>();

        while (true)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(responseContent);

            var articlesInPage = GetArticlesInPage(doc, board);

            articles = articlesInPage.Where(article => article.Date >= startDate).Concat(articles).ToList();

            var nextPageButton = doc.DocumentNode.SelectSingleNode("//div[@class='btn-group btn-group-paging']/a[contains(text(), '上頁')]");
            if (nextPageButton != null && articlesInPage.TrueForAll(a => a.Date >= startDate))
            {
                url = "https://www.ptt.cc" + nextPageButton.GetAttributeValue("href", "");
            }
            else
            {
                break;
            }
        }

        return articles;
    }
    
    private static List<Article> GetArticlesInPage(HtmlDocument doc, string board)
    {

        var rListContainer = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'r-list-container')]");
        if (rListContainer == null)
        {
            // can't find page
            return [];
        }
        var rListSep = rListContainer.SelectSingleNode(".//div[@class='r-list-sep']");
        
        var articlesInPage = new List<Article>();
        // TODO: bypass 18+ validation
        var rows = rListContainer.SelectNodes(".//div[@class='r-ent']")
            .Where(e => e.Line < rListSep.Line)
            .ToList();
        foreach (var row in rows)
        {
            try
            {
                var titleNode = row.SelectSingleNode(".//div[@class='title']/a");
                if (titleNode == null)
                {
                    continue;
                }
                var title = titleNode.InnerText.Trim();
                if (title.Contains("刪除"))
                {
                    continue;
                }

                var link = "https://www.ptt.cc" + titleNode.GetAttributeValue("href", "");
                var author = row.SelectSingleNode(".//div[@class='author']").InnerText.Trim();
                var dateStr = "2024/" + row.SelectSingleNode(".//div[@class='date']").InnerText.Trim();
                var date = DateTime.ParseExact(dateStr, "yyyy/M/d", CultureInfo.InvariantCulture).Date;

                articlesInPage.Add(new Article
                {
                    Board = board,
                    Title = title,
                    Link = link,
                    Date = date,
                    Author = author
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // Skip any articles that can't be parsed
            }
        }
        return articlesInPage;
    }
}