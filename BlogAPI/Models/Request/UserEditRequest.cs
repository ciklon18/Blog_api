namespace BlogAPI.Models.Request;

public record UserEditRequest(string Email, string FullName, string BirthDate, string Gender, string PhoneNumber)
{
    public UserEditRequest() : this(string.Empty, string.Empty, string.Empty, Enums.Gender.Male.ToString(), string.Empty)
    {
    }
}

