using BlogAPI.Enums;
using BlogAPI.Models.Request;
using BlogAPI.Models.Response;
using BlogAPI.services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    public async Task<PostPagedListResponse> GetPost(
        [FromQuery(Name = "tags")] List<Guid> tagIds,
        [FromQuery] string? authorName,
        [FromQuery] int minReadingTime,
        [FromQuery] int maxReadingTime,
        [FromQuery] PostSorting? sort,
        [FromQuery] bool isOnlyMyCommunities = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5
    )
    {
        return await _postService.GetPosts(tagIds, authorName, minReadingTime, maxReadingTime, sort, isOnlyMyCommunities,
            page, pageSize);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] PostRequest postRequest)
    {
        return await _postService.CreateUserPost(postRequest);
    }
	
    [HttpGet("{id}")]
    public async Task<PostResponse> GetPostById([FromRoute] Guid id)
    {
        return await _postService.GetPostById(id);
    }

    [HttpPost("{id}/like")]
    public async Task<IActionResult> LikePost([FromRoute] Guid id)
    {
        return await _postService.LikePost(id);
    }

    [HttpDelete("{id}/like")]
    public async Task<IActionResult> UnlikePost([FromRoute] Guid id)
    {
        return await _postService.UnlikePost(id);
    }
}