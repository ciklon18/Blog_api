namespace BlogAPI.Models.Response;

public record RefreshResponse(string AccessToken)
{
    public RefreshResponse() : this(string.Empty)
    {
        
    }
};