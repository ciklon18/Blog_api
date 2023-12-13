using BlogAPI.Enums;

namespace BlogAPI.Models.Request;
public record RegisterRequest(
    string FullName,
    string Password,
    string Email,
    DateTime BirthDate,
    Gender Gender,
    string? PhoneNumber)
{
    public RegisterRequest() : this(string.Empty, string.Empty, string.Empty, DateTime.Now.ToUniversalTime(), Gender.Male, string.Empty)
    {
    }
}