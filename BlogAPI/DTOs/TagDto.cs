namespace BlogAPI.Models.Response;

public record TagDto(Guid Id, DateTime CreateTime, string Name)
{
    public TagDto() : this(Guid.Empty, DateTime.MinValue, string.Empty)
    {
    }
}