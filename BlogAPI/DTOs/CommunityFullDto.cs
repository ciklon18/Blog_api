namespace BlogAPI.DTOs;
public record CommunityFullDto(Guid Id, DateTime CreateTime, string Name, string Description, bool IsClosed,
    int SubscribersCount, IEnumerable<UserDto> Administrators)
{
    public CommunityFullDto() : this(Guid.Empty, DateTime.Now, string.Empty, string.Empty, false, 0,
        new List<UserDto>())
    {
    }
}