namespace BlogAPI.Models.Request;

public record CreatePostDto(string Title, string Description, int ReadingTime, string? Image, Guid? AddressId, List<Guid> Tags);