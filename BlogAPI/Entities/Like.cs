using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogAPI.Entities;

[Table("likes")]
public class Like
{
    [Key] [Column("post_id")] public Guid PostId { get; set; }

    [Key] [Column("user_id")] public Guid UserId { get; set; }

    public Post Post { get; set; } = null!;

    public User User { get; set; } = null!;
}