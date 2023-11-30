using BlogAPI.Entities;

namespace BlogAPI.Models.Response;
public record CommunityFullResponse(Guid Id, DateTime CreateTime, string Name, string Description, bool IsClosed,
    int SubscribersCount, IEnumerable<User> Administrators)
{
    public CommunityFullResponse() : this(Guid.Empty, DateTime.Now, string.Empty, string.Empty, false, 0,
        new List<User>())
    {
    }
}