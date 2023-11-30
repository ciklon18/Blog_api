using BlogAPI.Enums;

namespace BlogAPI.Models.Response;

public record CommunityUserResponse(Guid UserId, Guid CommunityId, string Role)
{
    public CommunityUserResponse() : this(Guid.Empty, Guid.Empty, CommunityRole.Subscriber.ToString())
    {
    }
}