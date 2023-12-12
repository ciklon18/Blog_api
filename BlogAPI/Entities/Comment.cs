using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogAPI.Entities;

[Table("comments")]
public class Comment
{

    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("create_time")]
    public DateTime CreateTime { get; set; }

    [Required]
    [Column("content")]
    [MinLength(1)]
    public required string Content { get; set; }

    [Column("modified_date")]
    public DateTime? ModifiedDate { get; set; }

    [Column("delete_date")]
    public DateTime? DeleteDate { get; set; }

    [Required]
    [Column("author_id")]
    public Guid AuthorId { get; set; }

    [Required]
    [Column("author")]
    [MinLength(1)]
    public required string Author { get; set; }
    
    [Column("parent_id")]
    public Guid? ParentId { get; set; }
    
    [Column("post_id")]
    public required Guid PostId { get; set; }

    [Required]
    [Column("sub_comments")]
    public int SubComments { get; set; }
}