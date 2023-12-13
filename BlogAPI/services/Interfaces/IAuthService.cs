using BlogAPI.DTOs;
using BlogAPI.Models;
using BlogAPI.Models.Request;
using BlogAPI.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.services.Interfaces;

public interface IAuthService
{
    Task<RegistrationResponse> Register(UserRegisterModel userRegisterModel);
    Task<LoginResponse> Login(UserLoginModel userLoginModel);
    Task<IActionResult> Logout();
    RefreshResponse Refresh(UpdateRefreshDto updateRefreshDto);
}