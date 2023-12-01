namespace BlogAPI.Models.Response;
public record ChainAddressResponse(long ObjectId, string ObjectGuid, string? Text, string ObjectLevel, string ObjectLevelText)
{
    public ChainAddressResponse() : this(0, string.Empty, string.Empty, string.Empty, string.Empty)
    {
    }
};