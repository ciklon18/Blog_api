using BlogAPI.Enums;
using BlogAPI.Models.Request;
using BlogAPI.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.services.Interfaces;

public interface ICommunityService
{
    Task<List<CommunityResponse>> GetCommunityList();
    Task<List<CommunityUserResponse>> GetMyCommunityList();
    Task<CommunityFullResponse> GetCommunityInfo(Guid id);
    Task<PostPagedListResponse> GetCommunityPosts(Guid communityId, List<Guid> tagIds, PostSorting sort, int page, int pageSize);
    Task<IActionResult> PostCommunityPost(Guid communityId, PostRequest postRequest);
    Task<OkObjectResult> GetCommunityUserRole(Guid id);
    Task<IActionResult> SubscribeUserToCommunity(Guid communityId);
    Task<IActionResult> UnsubscribeUserToCommunity(Guid id);
}