using BlogAPI.Entities;
using BlogAPI.Enums;
using BlogAPI.Models.Request;
using BlogAPI.Models.Response;
using BlogAPI.services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.controllers;

[ApiController]
[Route("api/[controller]")]
public class CommunityController : ControllerBase
{
    private readonly ICommunityService _communityService;
    public CommunityController(ICommunityService communityService)
    {
        _communityService = communityService;
    }

    [HttpGet()]
    public async Task<List<CommunityResponse>> GetCommunityList()
    {
        return await _communityService.GetCommunityList();
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
    public async Task<PostPagedListResponse> GetCommunityPosts( 
        [FromRoute] Guid id,
        [FromQuery(Name = "tags")] List<Guid> tagIds,
        [FromQuery] PostSorting sort,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5)
    {
        return await _communityService.GetCommunityPosts(id, tagIds, sort, page, pageSize);
    }

    [HttpPost("{id}/post"), Authorize]
    public Task<IActionResult> PostCommunityPost([FromRoute] Guid id, [FromBody] PostRequest postRequest)
    {
        return _communityService.PostCommunityPost(id, postRequest);
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