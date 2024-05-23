using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace infrastructure.Models;

[Table("subscribed_board")]
public class SubscribedBoard : BaseModel
{
    [PrimaryKey("board")]
    public string Board { get; set; }

    [Column("last_latest_article_title")]
    public string LastLatestArticleTitle { get; set; }
}