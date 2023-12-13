namespace BlogAPI.DTOs;

public record CreateCommentDto(string Content, Guid? ParentId)
{
}