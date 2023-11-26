using BlogAPI.Models;
using BlogAPI.Models.Request;
using BlogAPI.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.services.Interfaces;

public interface IUserService
{
    public Task<UserProfileResponse> GetUserProfileAsync();
    public Task<IActionResult> UpdateUserProfileAsync(UserEditRequest userEditRequest);
}