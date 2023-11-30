using BlogAPI.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.services.Interfaces;

public interface ICommunityService
{
    List<CommunityResponse> GetCommunityList();
    Task<List<CommunityUserResponse>> GetMyCommunityList();
    Task<CommunityFullResponse> GetCommunityInfo(Guid id);
    Task<OkObjectResult>  GetCommunityUserRole(Guid id);
    Task<IActionResult> SubscribeUserToCommunity(Guid communityId);
    Task<IActionResult> UnsubscribeUserToCommunity(Guid id);
}