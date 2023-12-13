namespace BlogAPI.Models.Response;

public record CommunityDto(Guid Id, DateTime CreateTime, string Name, string Description, bool IsClosed,
    int SubscribersCount)
{
    public CommunityDto() : this(Guid.Empty, DateTime.Now, string.Empty, string.Empty, false, 0)
    {
    }
}