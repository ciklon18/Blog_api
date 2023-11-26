using BlogAPI.Entities;

namespace BlogAPI.Models.Response;

public record RegistrationResponse(string Email, string FullName)
{
    public RegistrationResponse() : this(string.Empty, string.Empty)
    {
        
    }
}