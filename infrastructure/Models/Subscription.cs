using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace infrastructure.Models;

[Table("subscriptions")]
public class Subscription : BaseModel
{
    [PrimaryKey("user_id")]
    public long UserId { get; set; }

    [PrimaryKey("board")]
    public string Board { get; set; }

    [PrimaryKey("keyword")]
    public string Keyword { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}