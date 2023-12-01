namespace BlogAPI.Models.Response;

public record CommunityResponse(Guid Id, DateTime CreateTime, string Name, string Description, bool IsClosed,
    int SubscribersCount)
{
    public CommunityResponse() : this(Guid.Empty, DateTime.Now, string.Empty, string.Empty, false, 0)
    {
    }
}