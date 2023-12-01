using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogAPI.Entities;

[Table("post_tags")]
public class PostTag
{
    [Key] [Column("post_id")] public Guid PostId { get; set; }

    [Key] [Column("tag_id")] public Guid TagId { get; set; }

    public Post Post { get; set; } = null!;

    public Tag Tag { get; set; } = null!;
}