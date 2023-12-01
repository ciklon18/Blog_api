namespace BlogAPI.Models.Request;

public record PostRequest(string Title, string Description, int ReadingTime, string? Image, Guid? AddressId, List<Guid> Tags)
{
    public PostRequest() : this("", "", 0, "", Guid.Empty, new List<Guid>())
    {
    }
}