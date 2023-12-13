
using BlogAPI.DTOs;
using BlogAPI.Models;
using BlogAPI.Models.Request;
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
    public async Task<IActionResult> Register([FromBody] UserRegisterModel userRegisterModel)
    {
        var newUser = await _authService.Register(userRegisterModel);
        return Ok(newUser);
    }

    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginModel userLoginModel)
    {
        var loginResponse = await _authService.Login(userLoginModel);
        return Ok(loginResponse);
    }


    [HttpPost("logout"), Authorize]
    public Task<IActionResult> Logout()
    {
        return _authService.Logout();
    }
    
    
    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] UpdateRefreshDto updateRefreshDto)
    {
        
        var refreshResponse = _authService.Refresh(updateRefreshDto);
        return Ok(refreshResponse);
    }
    
    
    [HttpGet("profile"), Authorize]
    public async Task<UserDto> GetProfile()
    {
        var userProfile = await _userService.GetUserProfileAsync();
        return userProfile;
    }
    
    
    [HttpPut("profile"), Authorize]
    public async Task<IActionResult> EditProfile([FromBody] UserEditModel userEditModel)
    {
        await _userService.UpdateUserProfileAsync(userEditModel);
        return Ok(new { message = "Profile updated successfully" });
    }
}