using System.Security.Claims;
using BlogAPI.Data;
using BlogAPI.Entities;
using BlogAPI.Exceptions;
using BlogAPI.Models.Request;
using BlogAPI.Models.Response;
using BlogAPI.services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.services.Impl;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _db;
    private readonly IJwtService _jwtService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(ApplicationDbContext db, IJwtService jwtService, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _jwtService = jwtService;
        _httpContextAccessor = httpContextAccessor;
    }


    public async Task<RegistrationResponse> Register(User user)
    {
        await IsUserUnique(user.Email);
        var newUser = CreateHashUser(user);
        await _db.Users.AddAsync(newUser);
        await _db.SaveChangesAsync();
        return new RegistrationResponse { Email = newUser.Email, FullName = newUser.FullName };
    }


    public async Task<LoginResponse> Login(LoginRequest loginRequest)
    {
        var user = await GetUserByEmailAsync(loginRequest.Email);
        CheckIsValidPassword(loginRequest.Password, user.Password);
        var existingRefreshToken = await _jwtService.GetRefreshTokenByEmailAsync(loginRequest.Email);
        var accessToken = _jwtService.GenerateAccessToken(user);
        
        var refreshToken = existingRefreshToken ?? _jwtService.GenerateRefreshToken(user);
        if (refreshToken != existingRefreshToken) await _jwtService.SaveRefreshTokenAsync(refreshToken, loginRequest.Email);
        
        return new LoginResponse { AccessToken = accessToken, RefreshToken = refreshToken };
    }


    public async Task<IActionResult> Logout()
    {
        var userEmail = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
        var refreshToken = await _jwtService.GetRefreshTokenByEmailAsync(userEmail);
        if (refreshToken == null) throw new NullTokenException("Refresh token not found");
        await _jwtService.RemoveRefreshTokenAsync(refreshToken);
        
        return new OkResult();
    }

    public async Task<RefreshResponse> Refresh(RefreshRequest refreshRequest)
    {
        var userEmail = _jwtService.GetEmailFromRefreshToken(refreshRequest.RefreshToken);
        await _jwtService.ValidateRefreshTokenAsync(refreshRequest.RefreshToken);
        var user = GetUserByEmail(userEmail);
        var accessToken = _jwtService.GenerateAccessToken(user);
        return new RefreshResponse { AccessToken = accessToken };
    }

    

    public async Task IsUserUnique(string? userEmail)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
        if (user != null) throw new UserAlreadyExistsException("User already exists");
    }


    private static void CheckIsValidPassword(string loginRequestPassword, string? userPassword)
    {
        if (!BCrypt.Net.BCrypt.Verify(loginRequestPassword, userPassword))
        {
            throw new IncorrectPasswordException("Wrong email or password");
        }
    }

    private async Task<User> GetUserByEmailAsync(string loginRequestEmail)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == loginRequestEmail);
        if (user == null) throw new UserNotFoundException("Wrong email or password");
        return user;
    }
    
    private User GetUserByEmail(string loginRequestEmail)
    {
        var user = _db.Users.FirstOrDefault(u => u.Email == loginRequestEmail);
        if (user == null) throw new UserNotFoundException("Wrong email or password");
        return user;
    }


    private static User CreateHashUser(User user)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
        return new User
        {
            FullName = user.FullName,
            Phone = user.Phone,
            Password = passwordHash,
            BirthDate = user.BirthDate.ToUniversalTime(),
            Email = user.Email,
            Gender = user.Gender,
            Id = user.Id,
            CreatedAt = DateTime.UtcNow.ToUniversalTime()
        };
    }

}