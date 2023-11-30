﻿using System.Security.Claims;
using BlogAPI.Data;
using BlogAPI.Entities;
using BlogAPI.Enums;
using BlogAPI.Exceptions;
using BlogAPI.Models.Response;
using BlogAPI.services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.services.Impl;

public class CommunityService : ICommunityService
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CommunityService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    public List<CommunityResponse> GetCommunityList()
    {
        var result = _db.Communities.ToList();
        return ConvertCommunitiesToCommunityResponseList(result);
    }


    public async Task<List<CommunityUserResponse>> GetMyCommunityList()
    {
        var userId = await GetUserGuidFromToken();

        var result = _db.UserCommunityRoles.Where(x => x.UserId == userId).ToList();
        return result.Select(x => new CommunityUserResponse(x.UserId, x.CommunityId, x.Role.ToString()))
            .ToList();
    }


    public async Task<CommunityFullResponse> GetCommunityInfo(Guid id)
    {
        var community = await _db.Communities.FirstOrDefaultAsync(community => community.Id == id);
        if (community == null) throw new CommunityNotFoundException("Community not found");
        var adminIds = await _db.UserCommunityRoles
            .Where(x => x.CommunityId == id && x.Role == CommunityRole.Administrator)
            .Select(x => x.UserId)
            .ToListAsync();
        var admins = await _db.Users.Where(x => adminIds.Contains(x.Id)).ToListAsync();
        
        return ConvertCommunityToCommunityFullResponse(community, admins);
    }


    public async Task<OkObjectResult> GetCommunityUserRole(Guid id)
    {
        await CheckIsThereCommunity(id);
        var userId = await GetUserGuidFromToken();
        var userCommunityRole =
            await _db.UserCommunityRoles.FirstOrDefaultAsync(x => x.UserId == userId && x.CommunityId == id);
        return new OkObjectResult(userCommunityRole?.Role is null ? "null" : userCommunityRole.Role);
    }


    public async Task<IActionResult> SubscribeUserToCommunity(Guid communityId)
    {
        var userId = await GetUserGuidFromToken();
        await CheckIsUserCommunityMember(userId, communityId);

        var community = await GetCommunity(communityId);
        AddSubscriberToCommunity(community, userId, communityId);
        return new OkResult();
    }


    public async Task<IActionResult> UnsubscribeUserToCommunity(Guid id)
    {
        var userId = await GetUserGuidFromToken();
        var userCommunityRole = await GetUserCommunityRole(userId, id);
        var community = await GetCommunity(id);
        await DeleteSubscriberFromCommunity(community, userCommunityRole);
        return new OkResult();
    }
    

    private async Task CheckIsUserCommunityMember(Guid userId, Guid communityId)
    {
        var userCommunityRole =
            await _db.UserCommunityRoles.FirstOrDefaultAsync(x => x.UserId == userId && x.CommunityId == communityId);
        if (userCommunityRole != null)
            throw new UserCommunityRoleAlreadyExistsException("User is already member of this community");
    }

    private async Task CheckIsThereCommunity(Guid id)
    {
        var community = await _db.Communities.FirstOrDefaultAsync(x => x.Id == id);
        if (community == null) throw new CommunityNotFoundException($"Community with id={id} not found in  database");
    }

    private void AddSubscriberToCommunity(Community community, Guid userId, Guid communityId)
    {
        community.SubscribersCount++;
        _db.UserCommunityRoles.Add(new UserCommunityRole
        {
            UserId = userId,
            CommunityId = communityId,
            Role = CommunityRole.Subscriber
        });
        _db.SaveChanges();
    }

    private async Task<UserCommunityRole> GetUserCommunityRole(Guid userId, Guid communityId)
    {
        var userCommunityRole =
            await _db.UserCommunityRoles.FirstOrDefaultAsync(x => x.UserId == userId && x.CommunityId == communityId);
        if (userCommunityRole == null) throw new UserCommunityRoleNotFoundException("User community role not found");
        return userCommunityRole;
    }

    private async Task<Community> GetCommunity(Guid id)
    {
        var community = await _db.Communities.FirstOrDefaultAsync(x => x.Id == id);
        if (community == null) throw new CommunityNotFoundException("Community not found");
        return community;
    }

    private async Task DeleteSubscriberFromCommunity(Community community, UserCommunityRole userCommunityRole)
    {
        community.SubscribersCount--;
        _db.UserCommunityRoles.Remove(userCommunityRole);
        await _db.SaveChangesAsync();
    }

    private static List<CommunityResponse> ConvertCommunitiesToCommunityResponseList(IEnumerable<Community> communities)
    {
        return communities.Select(community => new CommunityResponse(community.Id, community.CreateTime, community.Name,
                community.Description, community.IsClosed, community.SubscribersCount))
            .ToList();
    }

    private static CommunityFullResponse ConvertCommunityToCommunityFullResponse(Community community, List<User> administrators)
    {
        return new CommunityFullResponse
        {
            Administrators = administrators,
            CreateTime = community.CreateTime,
            Description = community.Description,
            Id = community.Id,
            IsClosed = community.IsClosed,
            Name = community.Name,
            SubscribersCount = community.SubscribersCount
        };
    }

    private async Task<Guid> GetUserGuidFromToken()
    {
        var userEmail = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == userEmail);
        if (user == null) throw new UserNotFoundException("User not found");
        return user.Id;
    }
}