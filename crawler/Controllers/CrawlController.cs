using infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace crawler.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CrawlController : Controller
{
    private readonly IPttClient _pttClient;

    public CrawlController(IPttClient pttClient)
    {
        _pttClient = pttClient;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(string keyword, string board, int days)
    {
        if (string.IsNullOrEmpty(keyword) || string.IsNullOrEmpty(board))
        {
            return BadRequest("Keyword, board parameters are required.");
        }

        var articles = (await _pttClient.SearchPttArticlesAsync(board, days))
            .Where(article => article.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase));

        return Ok(articles);
    }

}