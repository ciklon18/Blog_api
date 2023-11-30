namespace BlogAPI.Models.Response;

public record TagResponse(Guid Id, DateTime CreateTime, string Name)
{
    public TagResponse() : this(Guid.Empty, DateTime.MinValue, string.Empty)
    {
    }
}