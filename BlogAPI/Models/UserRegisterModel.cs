using BlogAPI.Enums;

namespace BlogAPI.Models.Request
{
    public record UserRegisterModel(
        string FullName,
        string Password,
        string Email,
        DateTime BirthDate,
        Gender Gender,
        string? PhoneNumber);
}