using BlogAPI.Entities;

namespace BlogAPI.Models.Response;

public record LoginResponse(string AccessToken, string RefreshToken)
{
    public LoginResponse() : this(string.Empty, string.Empty)
    {
        
    }
}