using BlogAPI.Entities;

namespace BlogAPI.services.Interfaces;

public interface IJwtService
{
    public string GenerateAccessToken(User user);
    public string GenerateRefreshToken(User user);
    public Task SaveRefreshTokenAsync(string refreshToken, string email);
    public Task RemoveRefreshTokenAsync(string refreshToken);
    public Task ValidateRefreshTokenAsync(string? token);
    public string GetEmailFromRefreshTokenAsync(string? token);
    public Task<string?> GetRefreshTokenByEmailAsync(string? email);
}