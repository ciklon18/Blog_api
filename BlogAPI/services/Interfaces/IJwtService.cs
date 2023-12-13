using BlogAPI.Entities;

namespace BlogAPI.services.Interfaces;

public interface IJwtService
{
    public string GenerateAccessToken(User user);
    public string GenerateRefreshToken(User user);
    public Task SaveRefreshTokenAsync(string refreshToken, Guid userId);
    public Task RemoveRefreshTokenAsync(string refreshToken);
    public Task ValidateRefreshTokenAsync(string? token);
    public void ValidateRefreshToken(string? token);
    public Guid GetGuidFromRefreshToken(string? token);
    public Task<string?> GetRefreshTokenByGuidAsync(Guid userId);
    public Task<Guid> GetUserGuidFromTokenAsync();
}