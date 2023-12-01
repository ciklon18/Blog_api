using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using BlogAPI.Converters;

namespace BlogAPI.Entities;

[Table("community")]
public class Community
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("create_time")]
    [JsonConverter(typeof(JsonDateTimeConverter))]
    public DateTime CreateTime { get; set; }
    
    [Column("name")]
    public string Name { get; set; } = null!;
    
    [Column("description")]
    public string Description { get; set; } = null!;
    
    [Column("is_closed")]
    public bool IsClosed { get; set; } = false;
    
    [Column("subscribers_count")]
    public int SubscribersCount { get; set; } = 0;
    
    [Column("administrators")]
    public IEnumerable<User> Administrators { get; set; } = new List<User>();
    
    public List<UserCommunityRole> UserCommunityRoles { get; set; } = new();
}