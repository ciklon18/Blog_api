namespace BlogAPI.Models.Response;

public record UserProfileResponse(string Id, string CreateTime, string FullName, string BirthDate, string Gender,
    string Email, string PhoneNumber)
{
    public UserProfileResponse() : this(string.Empty, string.Empty, string.Empty, 
        string.Empty, string.Empty, string.Empty, string.Empty)
    {
    }
}