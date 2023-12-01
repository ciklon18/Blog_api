namespace BlogAPI.Models.Response;

public record SearchAddressResponse(long ObjectId, Guid ObjectGuid, string? Text, string ObjectLevel, string ObjectLevelText)
{
    public SearchAddressResponse() : this(0, Guid.Empty, string.Empty, string.Empty, string.Empty)
    {
    }
}