using BlogAPI.DTOs;
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

    [HttpGet]
    public Task<List<CommunityDto>> GetCommunityList()
    {
        return _communityService.GetCommunityList();
    }

    [HttpGet("my"), Authorize]
    public Task<List<CommunityUserDto>> GetMyCommunityList()
    {
        return _communityService.GetMyCommunityList();
    }

    [HttpGet("{id}")]
    public Task<CommunityFullDto> GetCommunityInfo([FromRoute] Guid id)
    {
        return _communityService.GetCommunityInfo(id);
    }

    [HttpGet("{id}/post"), Authorize]
    public Task<PostPagedListDto> GetCommunityPosts( 
        [FromRoute] Guid id,
        [FromQuery(Name = "tags")] List<Guid> tagIds,
        [FromQuery] PostSorting sort,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5)
    {
        return _communityService.GetCommunityPosts(id, tagIds, sort, page, pageSize);
    }

    [HttpPost("{id}/post"), Authorize]
    public Task<IActionResult> PostCommunityPost([FromRoute] Guid id, [FromBody] CreatePostDto createPostDto)
    {
        return _communityService.PostCommunityPost(id, createPostDto);
    }

    [HttpGet("{id}/role"), Authorize]
    public Task<OkObjectResult> GetCommunityUserRole([FromRoute] Guid id)
    {
        return _communityService.GetCommunityUserRole(id);
    }

    [HttpPost("{id}/subscribe"), Authorize]
    public Task<IActionResult> SubscribeUserToCommunity([FromRoute] Guid id)
    {
        return _communityService.SubscribeUserToCommunity(id);
    }

    [HttpDelete("{id}/unsubscribe"), Authorize]
    public Task<IActionResult> UnsubscribeUserFromCommunity([FromRoute] Guid id)
    {
        return _communityService.UnsubscribeUserToCommunity(id);
    }

}