using System.Security.Claims;
using BlogAPI.Configurations;
using BlogAPI.Data;
using BlogAPI.Entities;
using BlogAPI.Enums;
using BlogAPI.Exceptions;
using BlogAPI.Models.Request;
using BlogAPI.Models.Response;
using BlogAPI.services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.services.Impl;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _db;
    private readonly IJwtService _jwtService;

    public UserService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext db, IJwtService jwtService)
    {
        _httpContextAccessor = httpContextAccessor;
        _db = db;
        _jwtService = jwtService;
    }

    public async Task<UserProfileResponse> GetUserProfileAsync()
    {
        var userEmail = GetUserEmail();
        if (userEmail == null) throw new UserNotFoundException("User not found");
        await CheckIsRefreshTokenValid(userEmail);
        var user = await GetUserByEmailAsync(userEmail);
        return EntityUserToUserDto(user);
    }


    public async Task<IActionResult> UpdateUserProfileAsync(UserEditRequest userEditRequest)
    {
        await CheckIsEmailInUseAsync(userEditRequest.Email);
        var userEmail = GetUserEmail();
        await CheckIsRefreshTokenValid(userEmail);
        await UpdateRefreshToken(userEmail);

        var user = await GetUserByEmailAsync(userEmail);
        _db.Users.Remove(user);
        var updatedUser = GetUpdatedUser(user, userEditRequest);
        _db.Users.Add(updatedUser);

        await _db.SaveChangesAsync();

        return new OkResult();
    }

    private async Task UpdateRefreshToken(string userEmail)
    {
        var refreshToken = await _jwtService.GetRefreshTokenByEmailAsync(userEmail);
        if (refreshToken == null) throw new InvalidRefreshToken("Refresh token not found");
        await _jwtService.RemoveRefreshTokenAsync(refreshToken);
        await _jwtService.SaveRefreshTokenAsync(refreshToken, userEmail);
    }


    private string GetUserEmail()
    {
        var userEmail = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
        if (userEmail == null) throw new UserNotFoundException("User not found");
        return userEmail;
    }

    private async Task<User> GetUserByEmailAsync(string email)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user == null) throw new UserNotFoundException("User not found");
        return user;
    }

    private async Task CheckIsRefreshTokenValid(string email)
    {
        var isEmailUsed = await _db.RefreshTokens.AnyAsync(u => u.Email == email);
        if (!isEmailUsed) throw new UnauthorizedException("Refresh token is not valid");
    }

    private static UserProfileResponse EntityUserToUserDto(User user)
    {
        return new UserProfileResponse
        {
            Id = user.Id.ToString(),
            CreateTime = user.CreatedAt.ToString(EntityConstants.DateTimeFormat),
            FullName = user.FullName,
            BirthDate = user.BirthDate.ToString(EntityConstants.DateTimeFormat),
            Gender = user.Gender.ToString(),
            Email = user.Email,
            PhoneNumber = user.Phone ?? string.Empty
        };
    }

    private static User GetUpdatedUser(User user, UserEditRequest userEditRequest)
    {
        return new User
        {
            Email = userEditRequest.Email,
            FullName = userEditRequest.FullName,
            BirthDate = DateTime.Parse(userEditRequest.BirthDate).ToUniversalTime(),
            Gender = ConvertStringToGender(userEditRequest.Gender),
            Phone = userEditRequest.PhoneNumber,
            CreatedAt = user.CreatedAt,
            Password = user.Password,
        };
    }

    private static Gender ConvertStringToGender(string gender)
    {
        return gender switch
        {
            "Male" => Gender.Male,
            "Female" => Gender.Female,
            _ => throw new IncorrectGenderException("Gender is incorrect")
        };
    }

    private async Task CheckIsEmailInUseAsync(string email)
    {
        var isEmailUsed = await _db.Users.AnyAsync(u => u.Email == email);
        if (isEmailUsed) throw new UserAlreadyExistsException("Email is already in use");
    }
}