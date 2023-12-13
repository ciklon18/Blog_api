using BlogAPI.Enums;

namespace BlogAPI.DTOs;

public record CommunityUserDto(Guid UserId, Guid CommunityId, string Role)
{
    public CommunityUserDto() : this(Guid.Empty, Guid.Empty, CommunityRole.Subscriber.ToString())
    {
    }
}