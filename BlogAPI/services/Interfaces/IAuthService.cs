using BlogAPI.Models.Request;
using BlogAPI.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.services.Interfaces;

public interface IAuthService
{
    Task<RegistrationResponse> Register(RegisterRequest registerRequest);
    Task<LoginResponse> Login(LoginRequest loginRequest);
    Task<IActionResult> Logout();
    RefreshResponse Refresh(RefreshRequest refreshRequest);
}