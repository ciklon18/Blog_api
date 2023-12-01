
using BlogAPI.Entities;
using BlogAPI.Models;
using BlogAPI.Models.Request;
using BlogAPI.Models.Response;
using BlogAPI.services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BlogAPI.controllers;


[ApiController]
[Route("api/account")]

public class UsersController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    
    
    public UsersController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }
    
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        var newUser = await _authService.Register(user);
        return Ok(newUser);
    }

    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var loginResponse = await _authService.Login(loginRequest);
        return Ok(loginResponse);
    }


    [HttpPost("logout"), Authorize]
    public async Task<IActionResult> Logout()
    {
        return await _authService.Logout();
    }
    
    
    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] RefreshRequest refreshRequest)
    {
        
        var refreshResponse = _authService.Refresh(refreshRequest);
        return Ok(refreshResponse);
    }
    
    
    [HttpGet("profile"), Authorize]
    public async Task<UserProfileResponse> GetProfile()
    {
        var userProfile = await _userService.GetUserProfileAsync();
        return userProfile;
    }
    
    
    [HttpPut("profile"), Authorize]
    public async Task<IActionResult> EditProfile([FromBody] UserEditRequest userEditRequest)
    {
        await _userService.UpdateUserProfileAsync(userEditRequest);
        return Ok(new { message = "Profile updated successfully" });
    }
}