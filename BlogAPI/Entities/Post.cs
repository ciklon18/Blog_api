using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BlogAPI.Entities;

[Table("posts")]
public class Post
{
    [Key] [Column("id")] public Guid Id { get; set; }

    [Column("create_time")] public DateTime CreateTime { get; set; }

    [Column("title")]
    [Required(AllowEmptyStrings = false)]
    public required string Title { get; set; }

    [Column("description")]
    [Required(AllowEmptyStrings = false)]
    public required string Description { get; set; }

    [Column("reading_time")] public int ReadingTime { get; set; }

    [Column("image")] public string? Image { get; set; }

    [Column("author_id")] public Guid AuthorId { get; set; }

    [Column("author")]
    [Required(AllowEmptyStrings = false)]
    public required string Author { get; set; }

    [Column("community_id")] public Guid? CommunityId { get; set; }

    [Column("community_name")]
    [Required(AllowEmptyStrings = false)]
    public required string CommunityName { get; set; }

    [Column("address_id")] public Guid? AddressId { get; set; }

    [Column("likes")] public int Likes { get; set; }

    [Column("has_like")] public bool HasLike { get; set; }

    [Column("comments_count")] public int CommentsCount { get; set; }

    
    [JsonIgnore]
    public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
}