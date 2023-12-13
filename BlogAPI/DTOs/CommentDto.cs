namespace BlogAPI.DTOs;

public record CommentDto(Guid Id, DateTime CreateTime, string Content, DateTime? ModifiedDate,
    DateTime? DeleteDate, Guid AuthorId, string Author, int SubComments)
{
    public CommentDto() : this(Guid.Empty, DateTime.MinValue, "", DateTime.MinValue, DateTime.MinValue,
        Guid.Empty, "", 0)
    {
    }
}