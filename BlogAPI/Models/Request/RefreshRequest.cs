namespace BlogAPI.Models.Request;

public record RefreshRequest(string RefreshToken)
{
    public RefreshRequest() : this(string.Empty)
    {
        
    }
};