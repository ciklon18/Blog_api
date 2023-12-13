namespace BlogAPI.DTOs;

public record CommunityDto(Guid Id, DateTime CreateTime, string Name, string Description, bool IsClosed,
    int SubscribersCount)
{
}