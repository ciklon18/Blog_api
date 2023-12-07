namespace BlogAPI.Models.Request;

public record UpdateCommentRequest(string Content)
{
    public UpdateCommentRequest() : this("")
    {
    }
}