namespace BlogAPI.Models.Response;


public record PostResponse(Guid Id, DateTime CreateTime, string Title, string Description, int ReadingTime,
    string? Image, Guid AuthorId, string Author, Guid? CommunityId, string? CommunityName, Guid? AddressId, int Likes,
    bool HasLike, int CommentsCount, List<TagResponse> Tags, List<CommentResponse> Comments)
{
    public PostResponse() : this(Guid.Empty, DateTime.MinValue, "", "", 0, "", Guid.Empty, "", Guid.Empty, "",
        Guid.Empty, 0, false, 0, new List<TagResponse>(), new List<CommentResponse>())
    {
    }
}