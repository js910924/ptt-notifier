using System.Globalization;
using HtmlAgilityPack;

namespace infrastructure.Models;

public class PttPageHtmlDocument(HtmlDocument doc, string board)
{
    public HtmlDocument Doc { get; private set; } = doc;
    public string Board { get; private set; } = board;

    public List<domain.Models.Article> GetArticlesInPage()
    {
        var rListContainer = Doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'r-list-container')]");
        if (rListContainer == null)
        {
            // can't find page
            return [];
        }
        var rListSep = rListContainer.SelectSingleNode(".//div[@class='r-list-sep']");
        
        var articlesInPage = new List<domain.Models.Article>();
        var rows = rListContainer.SelectNodes(".//div[@class='r-ent']")
            .Where(e => rListSep == null || e.Line < rListSep.Line)
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

                var link = PttClient.PttUrl + titleNode.GetAttributeValue("href", "");
                var author = row.SelectSingleNode(".//div[@class='author']").InnerText.Trim();
                // TODO: remove hard code 2024
                var dateStr = "2024/" + row.SelectSingleNode(".//div[@class='date']").InnerText.Trim();
                var date = DateTime.ParseExact(dateStr, "yyyy/M/d", CultureInfo.InvariantCulture).Date;

                articlesInPage.Add(new domain.Models.Article
                {
                    Board = Board,
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

    public string GetPreviousPageUrl()
    {
        var nextPageButton = Doc.DocumentNode.SelectSingleNode("//div[@class='btn-group btn-group-paging']/a[contains(text(), '上頁')]");
        return PttClient.PttUrl + nextPageButton.GetAttributeValue("href", "");
    }
}