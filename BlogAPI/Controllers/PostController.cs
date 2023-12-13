using BlogAPI.DTOs;
using BlogAPI.Enums;
using BlogAPI.Models.Request;
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
    public Task<PostPagedListDto> GetPost(
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
        return _postService.GetPosts(tagIds, authorName, minReadingTime, maxReadingTime, sort, isOnlyMyCommunities,
            page, pageSize);
    }

    [HttpPost()]
    public Task<IActionResult> CreatePost([FromBody] CreatePostDto createPostDto)
    {
        return _postService.CreateUserPost(createPostDto);
    }
	
    [HttpGet("{id}")]
    public Task<PostFullDto> GetPostById([FromRoute] Guid id)
    {
        return _postService.GetPostById(id);
    }

    [HttpPost("{postId}/like")]
    public Task<IActionResult> LikePost([FromRoute] Guid postId)
    {
        return _postService.LikePost(postId);
    }

    [HttpDelete("{postId}/like")]
    public Task<IActionResult> UnlikePost([FromRoute] Guid postId)
    {
        return _postService.UnlikePost(postId);
    }
}