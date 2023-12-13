using BlogAPI.Enums;

namespace BlogAPI.Models;
public record SearchAddressModel(long ObjectId, Guid ObjectGuid, string? Text, ObjectLevel ObjectLevel, string ObjectLevelText)
{
    public SearchAddressModel() : this(0, Guid.Empty, string.Empty, ObjectLevel.Building, string.Empty)
    {
    }
};