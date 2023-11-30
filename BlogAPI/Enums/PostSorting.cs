using System.Text.Json.Serialization;

namespace BlogAPI.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PostSorting
{
    CreateDesc,
    LikeDesc,
    CreateAsc,
    LikeAsc
}
