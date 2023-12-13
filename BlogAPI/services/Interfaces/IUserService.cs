using BlogAPI.DTOs;
using BlogAPI.Entities;
using BlogAPI.Models.Request;
using BlogAPI.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.services.Interfaces;

public interface IUserService
{
    public Task<UserDto> GetUserProfileAsync();
    public Task<IActionResult> UpdateUserProfileAsync(UserEditRequest userEditRequest);
}