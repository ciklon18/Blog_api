using BlogAPI.Enums;
using BlogAPI.Models.Response;
using BlogAPI.services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.controllers;

[ApiController]
[Route("api/[controller]")]
public class CommunityController
{
    private readonly ICommunityService _communityService;
    public CommunityController(ICommunityService communityService)
    {
        _communityService = communityService;
    }

    [HttpGet()]
    public List<CommunityResponse> GetCommunityList()
    {
        return _communityService.GetCommunityList();
    }

    [HttpGet("my"), Authorize]
    public async Task<List<CommunityUserResponse>> GetMyCommunityList()
    {
        return await _communityService.GetMyCommunityList();
    }

    [HttpGet("{id}")]
    public async Task<CommunityFullResponse> GetCommunityInfo([FromRoute] Guid id)
    {
        return await _communityService.GetCommunityInfo(id);
    }

    [HttpGet("{id}/post"), Authorize]
    public IActionResult GetCommunityPosts( //TODO change return type 
        [FromRoute] Guid id,
        [FromQuery(Name = "tags")] List<Guid> tagIds,
        // change available sorting options to PostSorting enum
        [FromQuery] PostSorting sort,
        [FromQuery] int page,
        [FromQuery] int pageSize)
    {
        return new OkObjectResult("Hello World");
    }

    [HttpPost("{id}/post"), Authorize]
    public IActionResult PostCommunityPost([FromRoute] Guid id) //TODO change return type 
    {
        return new OkObjectResult("Hello World");
    }

    [HttpGet("{id}/role"), Authorize]
    public async Task<OkObjectResult> GetCommunityUserRole([FromRoute] Guid id)
    {
        var userRole = await _communityService.GetCommunityUserRole(id);
        return userRole;
    }

    [HttpPost("{id}/subscribe"), Authorize]
    public async Task<IActionResult> SubscribeUserToCommunity([FromRoute] Guid id)
    {
        return await _communityService.SubscribeUserToCommunity(id);
    }

    [HttpDelete("{id}/unsubscribe"), Authorize]
    public async Task<IActionResult> UnsubscribeUserFromCommunity([FromRoute] Guid id)
    {
        return await _communityService.UnsubscribeUserToCommunity(id);
    }

}