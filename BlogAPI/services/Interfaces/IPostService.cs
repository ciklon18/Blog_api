using BlogAPI.DTOs;
using BlogAPI.Entities;
using BlogAPI.Enums;
using BlogAPI.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.services.Interfaces;

public interface IPostService
{
    Task<PostPagedListDto> GetPosts(List<Guid> tagIds, string? authorName, int minReadingTime, int maxReadingTime, PostSorting? sort, bool isOnlyMyCommunities, int page, int pageSize);
    Task<IActionResult> CreateUserPost(PostRequest postRequest);
    Task<PostFullDto> GetPostById(Guid postId);
    Task<IActionResult> LikePost(Guid postId);
    Task<IActionResult> UnlikePost(Guid postId);
    Task<IActionResult> CreateCommunityPost(Guid communityId, string? communityName, PostRequest postRequest);
    Task<PostPagedListDto> ConvertPostsToPostPagedListResponse(IQueryable<PostDto> posts, int page, int pageSize);
    IQueryable<PostDto> ConvertPostsToPostDtoList(IEnumerable<Post> posts);
    IQueryable<PostDto> GetFilteredAndSortedCommunityPosts(IQueryable<PostDto> posts, List<Guid> tagIds, PostSorting sort, int page, int pageSize);
}