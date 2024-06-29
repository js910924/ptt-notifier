using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace infrastructure.Models;

[Table("subscriptions")]
public class Subscription : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public long UserId { get; set; }

    [Column("board")]
    public string Board { get; set; }

    [Column("keyword")]
    public string? Keyword { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("author")]
    public string? Author { get; set; }
}