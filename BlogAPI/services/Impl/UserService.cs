﻿using BlogAPI.Data;
using BlogAPI.DTOs;
using BlogAPI.Entities;
using BlogAPI.Enums;
using BlogAPI.Exceptions;
using BlogAPI.Models;
using BlogAPI.services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.services.Impl;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _db;
    private readonly IJwtService _jwtService;

    public UserService(ApplicationDbContext db, IJwtService jwtService)
    {
        _db = db;
        _jwtService = jwtService;
    }

    public async Task<UserDto> GetUserProfileAsync()
    {
        var userId = await GetUserGuid();
        await CheckIsRefreshTokenValid(userId);
        var user = await GetUserByGuidAsync(userId);
        return EntityUserToUserDto(user);
    }


    public async Task<IActionResult> UpdateUserProfileAsync(UserEditModel userEditModel)
    {
        
        var userGuid = await GetUserGuid();
        
        await CheckIsEmailInUseAsync(userEditModel.Email, userGuid);
        await CheckIsRefreshTokenValid(userGuid);

        var user = await GetUserByGuidAsync(userGuid);
        var updatedUser = GetUpdatedUser(user, userEditModel);
        
        _db.Users.Remove(user);
        _db.Users.Add(updatedUser);
        
        await _db.SaveChangesAsync();
        

        return new OkResult();
    }
    

    private async Task<Guid> GetUserGuid()
    {
        var userId = await _jwtService.GetUserGuidFromTokenAsync();
        if (userId == Guid.Empty) throw new UserNotFoundException("User not found");
        return userId;
    }


    private async Task<User> GetUserByGuidAsync(Guid userId )
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Id == userId);
        if (user == null) throw new UserNotFoundException("User not found");
        return user;
    }

    private async Task CheckIsRefreshTokenValid(Guid userId)
    {
        var isGuidUsed = await _db.RefreshTokens.AnyAsync(u => u.UserId == userId);
        if (!isGuidUsed) throw new UnauthorizedException("Refresh token is not valid");
    }

    private static UserDto EntityUserToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            CreateTime = user.CreatedAt.ToUniversalTime(),
            FullName = user.FullName,
            BirthDate = user.BirthDate.ToUniversalTime(),
            Gender = user.Gender,
            Email = user.Email,
            PhoneNumber = user.Phone ?? string.Empty
        };
    }

    private static User GetUpdatedUser(User user, UserEditModel userEditModel)
    {
        return new User
        {
            Id = user.Id,
            Email = userEditModel.Email != "" ? userEditModel.Email : user.Email,
            FullName = userEditModel.FullName != "" ? userEditModel.FullName : user.FullName,
            BirthDate = DateTime.Parse(userEditModel.BirthDate).ToUniversalTime(),
            Gender = ConvertStringToGender(userEditModel.Gender),
            Phone = userEditModel.PhoneNumber != "" ? userEditModel.PhoneNumber : user.Phone,
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

    private async Task CheckIsEmailInUseAsync(string email, Guid userId)
    {
        var isEmailUsed = await _db.Users.AnyAsync(u => u.Email == email && u.Id != userId);
        if (isEmailUsed) throw new UserAlreadyExistsException("Email is already in use");
    }
}