using BlogAPI.Entities;
using BlogAPI.Models.Request;
using BlogAPI.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.services.Interfaces;

public interface IAuthService
{
    Task<RegistrationResponse> Register(User user);
    Task<LoginResponse> Login(LoginRequest loginRequest);
    Task<IActionResult> Logout();
    Task<RefreshResponse> Refresh(RefreshRequest refreshRequest);
}