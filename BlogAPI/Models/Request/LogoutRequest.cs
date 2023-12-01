namespace BlogAPI.Models.Request;

public record LogoutRequest(string RefreshToken)
{
    public LogoutRequest() : this(string.Empty)
    {
        
    }
};