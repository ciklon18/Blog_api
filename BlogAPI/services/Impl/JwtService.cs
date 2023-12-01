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

    public JwtService(JwtSecurityTokenHandler tokenHandler, Tokens tokens, ApplicationDbContext db)
    {
        _tokenHandler = tokenHandler;
        _tokens = tokens;
        _db = db;
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

    public async Task SaveRefreshTokenAsync(string refreshToken, string email)
    {
        var expiration = DateTime.UtcNow.AddMinutes(_tokens.RefreshTokenExpiration.TotalMinutes);
        _db.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            Email = email,
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
                new Claim(ClaimTypes.Email, user?.Email ?? ""),
            }),
            Expires = DateTime.UtcNow.AddMinutes(expirationTime),
            SigningCredentials =
                new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };
        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }


    public async Task ValidateRefreshTokenAsync(string? token)
    {
        CheckTokenNotNull(token);
        var refreshToken =  _db.RefreshTokens.FirstOrDefault(t => t.Token == token);
        CheckTokenNotRevoked(refreshToken);
        CheckTokenNotExpired(refreshToken);
    }
    
    public string GetEmailFromRefreshToken(string? token)
    {
        CheckTokenNotNull(token);
        var refreshToken =  _db.RefreshTokens.FirstOrDefault(t => t.Token == token);
        CheckTokenNotRevoked(refreshToken);
        CheckTokenNotExpired(refreshToken);
        if (refreshToken == null) throw new NullTokenException("GetEmailFromRefreshTokenAsync: Refresh token is null");
        return refreshToken.Email;
    }

    public async Task<string?> GetRefreshTokenByEmailAsync(string? email)
    {
        CheckEmailNotNull(email);

        var refreshToken = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Email == email);
        if (refreshToken == null) return null;
        CheckTokenNotRevoked(refreshToken);
        CheckTokenNotExpired(refreshToken);
        return refreshToken.Token;
    }

    private static void CheckTokenNotNull(string? token)
    {
        if (token == null) throw new NullTokenException("Refresh token is null");
    }

    private static void CheckEmailNotNull(string? email)
    {
        if (email == null) throw new NullEmailException("Email is null");
    }

    private static void CheckTokenNotRevoked(RefreshToken? refreshToken)
    {
        if (refreshToken is { Revoked: true })
            throw new InvalidTokenException("Invalid or revoked refresh token");
    }

    private static void CheckTokenNotExpired(RefreshToken? refreshToken)
    {
        if (refreshToken is { IsExpired: true })
            throw new ExpiredRefreshTokenException(
                $"Refresh token is expired. Expiration date: {refreshToken.Expires}");
    }
}