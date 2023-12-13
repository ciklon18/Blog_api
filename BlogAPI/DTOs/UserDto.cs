using BlogAPI.Enums;

namespace BlogAPI.DTOs;


public record UserDto(
    Guid Id,
    DateTime CreateTime,
    string FullName,
    DateTime? BirthDate,
    Gender Gender,
    string Email,
    string? PhoneNumber)
{
    public UserDto() : this(Guid.Empty, DateTime.Now.ToUniversalTime(), string.Empty, DateTime.Now.ToUniversalTime(),
        Gender.Male, string.Empty, string.Empty)
    {
    }
}