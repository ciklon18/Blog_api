using System.Text.RegularExpressions;
using BlogAPI.Configurations;
using BlogAPI.Data;
using BlogAPI.Entities;
using BlogAPI.Exceptions;
using BlogAPI.Models;
using BlogAPI.Models.Request;
using BlogAPI.Models.Response;
using BlogAPI.services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.services.Impl;

public partial class AuthService : IAuthService
{
    private readonly ApplicationDbContext _db;
    private readonly IJwtService _jwtService;

    public AuthService(ApplicationDbContext db, IJwtService jwtService)
    {
        _db = db;
        _jwtService = jwtService;
    }


    public async Task<RegistrationResponse> Register(UserRegisterModel userRegisterModel)
    {
        await IsUserUnique(userRegisterModel.Email);
        await ValidateRegisterData(userRegisterModel);
        var newUser = CreateHashUser(userRegisterModel);
        await _db.Users.AddAsync(newUser);
        await _db.SaveChangesAsync();
        return new RegistrationResponse { Email = newUser.Email, FullName = newUser.FullName };
    }



    public async Task<LoginResponse> Login(UserLoginModel userLoginModel)
    {
        var user = await GetUserByEmailAsync(userLoginModel.Email);
        CheckIsValidPassword(userLoginModel.Password, user.Password);
        var existingRefreshToken = await _jwtService.GetRefreshTokenByGuidAsync(user.Id);
        var accessToken = _jwtService.GenerateAccessToken(user);
        
        var refreshToken = existingRefreshToken ?? _jwtService.GenerateRefreshToken(user);
        if (refreshToken != existingRefreshToken) await _jwtService.SaveRefreshTokenAsync(refreshToken, user.Id);
        
        return new LoginResponse { AccessToken = accessToken, RefreshToken = refreshToken };
    }


    public async Task<IActionResult> Logout()
    {
        var userId = await _jwtService.GetUserGuidFromTokenAsync();
        if (userId == Guid.Empty) throw new UserNotFoundException("User not found");
        var refreshToken = await _jwtService.GetRefreshTokenByGuidAsync(userId);
        if (refreshToken == null) throw new NullTokenException("Refresh token not found");
        await _jwtService.RemoveRefreshTokenAsync(refreshToken);
        
        return new OkResult();
    }

    public RefreshResponse Refresh(RefreshRequest refreshRequest)
    {
        var userGuid = _jwtService.GetGuidFromRefreshToken(refreshRequest.RefreshToken);
        _jwtService.ValidateRefreshToken(refreshRequest.RefreshToken);
        var user = GetUserByGuid(userGuid);
        var accessToken = _jwtService.GenerateAccessToken(user);
        return new RefreshResponse { AccessToken = accessToken };
    }

    

    private async Task IsUserUnique(string? userEmail)
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
    private User GetUserByGuid(Guid userId)
    {
        var user =  _db.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null) throw new UserNotFoundException("User not found");
        return user;
    }


    private static User CreateHashUser(UserRegisterModel userRegisterModel)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(userRegisterModel.Password);
        return new User
        {
            FullName = userRegisterModel.FullName,
            Phone = userRegisterModel.PhoneNumber,
            Password = passwordHash,
            BirthDate = userRegisterModel.BirthDate.ToUniversalTime(),
            Email = userRegisterModel.Email,
            Gender = userRegisterModel.Gender,
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow.ToUniversalTime()
        };
    }
    
    private static Task ValidateRegisterData(UserRegisterModel userRegisterModel)
    {
        if (userRegisterModel.PhoneNumber != null && !PhoneNumberRegex().IsMatch(userRegisterModel.PhoneNumber))
        {
            throw new IncorrectPhoneException(EntityConstants.IncorrectPhoneNumberError);
        }
        if (!PasswordRegex().IsMatch(userRegisterModel.Password))
        {
            throw new IncorrectRegisterDataException(EntityConstants.IncorrectPassword);
        }

        if (!EmailRegex().IsMatch(userRegisterModel.Email))
        {
            throw new IncorrectRegisterDataException(EntityConstants.IncorrectEmailError);
        }

        return Task.CompletedTask;
    }
    
    


    
    [GeneratedRegex(pattern: EntityConstants.PhoneNumberRegex)]
    private static partial Regex PhoneNumberRegex();
    
    [GeneratedRegex(pattern: EntityConstants.EmailRegex)]
    private static partial Regex EmailRegex();    
    [GeneratedRegex(pattern: EntityConstants.PasswordRegex)]
    private static partial Regex PasswordRegex();
}