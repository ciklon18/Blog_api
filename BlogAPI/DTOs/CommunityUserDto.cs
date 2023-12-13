namespace BlogAPI.DTOs;

public record CommunityUserDto(Guid UserId, Guid CommunityId, string Role);