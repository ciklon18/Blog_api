using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BlogAPI.Configurations;
using BlogAPI.Data;
using BlogAPI.Entities;
using BlogAPI.Exceptions;
using BlogAPI.services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BlogAPI.services.Impl;

public class JwtService : IJwtService
{
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly Tokens _tokens;
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtService(JwtSecurityTokenHandler tokenHandler, Tokens tokens, ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
    {
        _tokenHandler = tokenHandler;
        _tokens = tokens;
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    public string GenerateAccessToken(User? user)
    {
        return GenerateToken(user: user, key: _tokens.AccessTokenKey,
            expirationTime: _tokens.AccessTokenExpiration.TotalMinutes);
    }

    public string GenerateRefreshToken(User? user)
    {
        return GenerateToken(user: user, key: _tokens.RefreshTokenKey,
            expirationTime: _tokens.RefreshTokenExpiration.TotalMinutes);
    }
    

    public async Task SaveRefreshTokenAsync(string refreshToken, Guid userId)
    {
        var expiration = DateTime.UtcNow.AddMinutes(_tokens.RefreshTokenExpiration.TotalMinutes);
        _db.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            UserId = userId,
            Expires = expiration
        });
        await _db.SaveChangesAsync();
    }

    public async Task RemoveRefreshTokenAsync(string refreshToken)
    {
        _db.RefreshTokens.RemoveRange(_db.RefreshTokens.Where(t => t.Token == refreshToken));
        await _db.SaveChangesAsync();
    }


    private string GenerateToken(User? user, SecurityKey? key, double expirationTime)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user?.Id.ToString() ?? string.Empty),
            }),
            Expires = DateTime.UtcNow.AddMinutes(expirationTime),
            SigningCredentials =
                new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };
        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }


    public async Task<Guid> GetUserGuidFromTokenAsync()
    {
        var stringUserId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userGuid = Guid.Parse(stringUserId ?? string.Empty);
        if (userGuid == Guid.Empty) throw new UserNotFoundException("User not found");
        await CheckIsRefreshTokenValid(userGuid);
        var user = await _db.Users.FirstOrDefaultAsync(user => user.Id == userGuid);
        if (user == null) throw new UserNotFoundException("User not found");
        return user.Id;
    }

    private async Task CheckIsRefreshTokenValid(Guid userId)
    {
        var isGuidUsed = await _db.RefreshTokens.AnyAsync(token => token.UserId == userId);
        if (!isGuidUsed) throw new UnauthorizedException("Refresh token is not valid");
    }


    public async Task ValidateRefreshTokenAsync(string? token)
    {
        CheckTokenNotNull(token);
        var refreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);
        if (refreshToken == null) throw new NullTokenException("Refresh token is null");
        CheckTokenNotRevokedAndNotExpired(refreshToken);
    }
    public void ValidateRefreshToken(string? token)
    {
        CheckTokenNotNull(token);
        var refreshToken = _db.RefreshTokens.FirstOrDefault(t => t.Token == token);
        if (refreshToken == null) throw new NullTokenException("Refresh token is null");
        CheckTokenNotRevokedAndNotExpired(refreshToken);
    }
    


    public Guid GetGuidFromRefreshToken(string? token)
    {
        CheckTokenNotNull(token);
        var refreshToken = _db.RefreshTokens.FirstOrDefault(t => t.Token == token);
        if (refreshToken == null) throw new NullTokenException("Refresh token is null");
        CheckTokenNotRevokedAndNotExpired(refreshToken);
        
        return refreshToken.UserId;
    }


    public async Task<string?> GetRefreshTokenByGuidAsync(Guid userId)
    {
        CheckGuidNotNull(userId);
        var refreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.UserId == userId);
        return refreshToken?.Token;
    }

    private static void CheckTokenNotNull(string? token)
    {
        if (token == null) throw new NullTokenException("Refresh token is null");
    }

    private static void CheckGuidNotNull(Guid? userId)
    {
        if (userId == null) throw new NullEmailException("Id is null");
    }

    private static void CheckTokenNotRevokedAndNotExpired(RefreshToken? refreshToken)
    {
        switch (refreshToken)
        {
            case { Revoked: true }:
                throw new InvalidTokenException("Invalid or revoked refresh token");
            case { IsExpired: true }:
                throw new ExpiredRefreshTokenException(
                    $"Refresh token is expired. Expiration date: {refreshToken.Expires}");
        }
    }

}