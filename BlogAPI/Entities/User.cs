using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using BlogAPI.Configurations;
using BlogAPI.Converters;
using BlogAPI.Enums;

namespace BlogAPI.Entities;

[Table("users")]
public class User
{
    [Key] [Column("id")] public Guid Id { get; set; }

    [Column("full_name")]
    [RegularExpression(pattern: EntityConstants.FullNameRegex,
        ErrorMessage = EntityConstants.WrongSymbolInFullNameError)]
    public required string FullName { get; set; }

    [Column("birth_date")]
    [JsonConverter(typeof(JsonDateTimeConverter))]
    public DateTime BirthDate { get; set; }

    [Column("email")]
    [EmailAddress(ErrorMessage = EntityConstants.IncorrectEmailError)]
    public required string Email { get; set; }
    

    [Column("phone")]
    [RegularExpression(pattern: EntityConstants.PhoneNumberRegex,
        ErrorMessage = EntityConstants.IncorrectPhoneNumberError)]
    public string? Phone { get; set; }

    [Column("gender")] 
    [EnumDataType(typeof(Gender))]
    public string? Gender { get; set; }

    [Column("password")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = EntityConstants.ShortPasswordError)]
    public string? Password { get; set; }
    
    [Required]
    [Column("created_at")]
    [JsonConverter(typeof(JsonDateTimeConverter))]
    public DateTime CreatedAt { get; set; }
    
    public List<UserCommunityRole> UserCommunityRoles { get; set; } = new();
}