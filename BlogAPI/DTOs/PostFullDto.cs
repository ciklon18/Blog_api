namespace BlogAPI.DTOs;

public record PostFullDto(Guid Id, DateTime CreateTime, string Title, string Description, int ReadingTime,
    string? Image, Guid AuthorId, string Author, Guid? CommunityId, string? CommunityName, Guid? AddressId, int Likes,
    bool HasLike, int CommentsCount, List<TagDto> Tags, List<CommentDto> Comments);