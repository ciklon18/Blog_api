namespace BlogAPI.Models.Response;

public record CommentResponse(Guid Id, DateTime CreateTime, string Content, DateTime? ModifiedDate,
    DateTime? DeleteDate, Guid AuthorId, string Author, int SubComments)
{
    public CommentResponse() : this(Guid.Empty, DateTime.MinValue, "", DateTime.MinValue, DateTime.MinValue,
        Guid.Empty, "", 0)
    {
    }
}