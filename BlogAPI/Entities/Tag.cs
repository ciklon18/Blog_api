using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using BlogAPI.Converters;

namespace BlogAPI.Entities;

[Table("tags")]
public class Tag
{

    [Column("Id")]
    public Guid Id { get; set; }
    
    [JsonConverter(typeof(JsonDateTimeConverter))]
    [Column("CreateTime")]
    public DateTime CreateTime { get; set; }
    
    [Column("Name")]
    public string Name { get; set; } = string.Empty;
    
    
}