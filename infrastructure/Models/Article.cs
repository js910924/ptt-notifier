using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace infrastructure.Models;

[Table("articles")]
public class Article : BaseModel
{
    [PrimaryKey("id")]
    public long Id { get; set; }

    [Column("board")]
    public string Board { get; set; }

    [Column("title")]
    public string Title { get; set; }

    [Column("link")]
    public string Link { get; set; }

    [Column("author")]
    public string Author { get; set; }
}