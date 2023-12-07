namespace BlogAPI.Models.Request;

public record CreateCommentRequest(string Content, Guid? ParentId)
{
    public CreateCommentRequest() : this("", Guid.Empty)
    {
    }
}