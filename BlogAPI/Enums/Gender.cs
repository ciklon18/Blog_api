using System.Text.Json.Serialization;

namespace BlogAPI.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Gender
{
    Male,
    Female
}