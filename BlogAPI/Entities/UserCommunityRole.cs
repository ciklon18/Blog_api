using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlogAPI.Enums;

namespace BlogAPI.Entities;

[Table("user_community_role")]

public class UserCommunityRole
{
    [Key]
    [Column("user_id")]
    public Guid UserId { get; set; }
    
    [Key]
    [Column("community_id")]
    public Guid CommunityId { get; set; }
    
    [Column("role")]
    public CommunityRole Role { get; set; }
    
    public User User { get; set; } = null!;
    
    public Community Community { get; set; } = null!;
}