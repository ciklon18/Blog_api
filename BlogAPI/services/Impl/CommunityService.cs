﻿using BlogAPI.Data;
using BlogAPI.DTOs;
using BlogAPI.Entities;
using BlogAPI.Enums;
using BlogAPI.Exceptions;
using BlogAPI.Models.Request;
using BlogAPI.Models.Response;
using BlogAPI.services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.services.Impl;

public class CommunityService : ICommunityService
{
    private readonly ApplicationDbContext _db;
    private readonly IPostService _postService;
    private readonly IJwtService _jwtService;

    public CommunityService(ApplicationDbContext db, IJwtService jwtService, IPostService postService)
    {
        _db = db;
        _jwtService = jwtService;
        _postService = postService;
    }

    public async Task<List<CommunityDto>> GetCommunityList()
    {
        var result = await _db.Communities.ToListAsync();
        return ConvertCommunitiesToCommunityResponseListAsync(result);
    }


    public async Task<List<CommunityUserDto>> GetMyCommunityList()
    {
        var userId = await _jwtService.GetUserGuidFromTokenAsync();
        var result = _db.UserCommunityRoles.Where(x => x.UserId == userId).ToList();
        return result.Select(x => new CommunityUserDto(x.UserId, x.CommunityId, x.Role.ToString()))
            .ToList();
    }


    public async Task<CommunityFullDto> GetCommunityInfo(Guid id)
    {
        var community = await _db.Communities.FirstOrDefaultAsync(community => community.Id == id);
        if (community == null) throw new CommunityNotFoundException("Community not found");
        var adminIds = await GetCommunityAdminIds(id);
        var admins = await GetCommunityAdministrators(adminIds);

        return ConvertCommunityToCommunityFullResponse(community, admins);
    }

    private async Task<List<UserDto>> GetCommunityAdministrators(ICollection<Guid> adminIds)
    {
        var users = await _db.Users.Where(x => adminIds.Contains(x.Id)).ToListAsync();
        return ConvertUsersToUserDtoList(users);
    }

    private static List<UserDto> ConvertUsersToUserDtoList(IEnumerable<User> users)
    {
        return users.Select(user => new UserDto
            {
                Id = user.Id,
                BirthDate = user.BirthDate,
                CreateTime = user.CreatedAt,
                Email = user.Email,
                Gender = user.Gender,
                FullName = user.FullName,
                PhoneNumber = user.Phone
            })
            .ToList();
    }


    public async Task<PostPagedListDto> GetCommunityPosts(Guid communityId, List<Guid> tagIds, PostSorting sort,
        int page, int pageSize)
    {
        await CheckIsThereCommunity(communityId);
        CheckIsPaginationValid(page, pageSize);
        var userId = await _jwtService.GetUserGuidFromTokenAsync();
        if (await IsCommunityClosed(communityId))
            await CheckIsUserSubscribedToCommunity(communityId, userId);
        var posts = _postService.ConvertPostsToPostDtoList(_db.Posts.Where(x => x.CommunityId == communityId));
        posts = _postService.GetFilteredAndSortedCommunityPosts(posts, tagIds, sort, page, pageSize);
        return await _postService.ConvertPostsToPostPagedListResponse(posts, page, pageSize);
    }

    private static void CheckIsPaginationValid(int page, int pageSize)
    {
        if (page < 1) throw new InvalidPaginationException("Invalid value for attribute page");
        if (pageSize < 1) throw new InvalidPaginationException("Invalid value for attribute pageSize");
    }

    private Task<bool> IsCommunityClosed(Guid communityId)
    {
        return _db.Communities.Where(x => x.Id == communityId).Select(x => x.IsClosed).FirstOrDefaultAsync();
    }

    private async Task CheckIsUserSubscribedToCommunity(Guid communityId, Guid userId)
    {
        var userCommunityRole =
            await _db.UserCommunityRoles.FirstOrDefaultAsync(x => x.UserId == userId && x.CommunityId == communityId);
        if (userCommunityRole == null)
            throw new ForbiddenAccessToClosedCommunityException(
                $"Access to closed community with id={communityId} is forbidden");
    }


    private async Task CheckIsUserCommunityAdministrator(Guid userId, Guid communityId)
    {
        var userCommunityRole =
            await _db.UserCommunityRoles.FirstOrDefaultAsync(x => x.UserId == userId && x.CommunityId == communityId);
        if (userCommunityRole?.Role != CommunityRole.Administrator)
            throw new UserCommunityRoleNotFoundException("User does not have administrator role in this community");
    }


    private Task<string?> GetCommunityNameWithCommunityId(Guid communityId)
    {
        return _db.Communities
            .Where(x => x.Id == communityId)
            .Select(x => x.Name)
            .FirstOrDefaultAsync();
    }

    public async Task<OkObjectResult> GetCommunityUserRole(Guid id)
    {
        await CheckIsThereCommunity(id);
        var userId = await _jwtService.GetUserGuidFromTokenAsync();
        var userCommunityRole =
            await _db.UserCommunityRoles.FirstOrDefaultAsync(x => x.UserId == userId && x.CommunityId == id);
        return new OkObjectResult(userCommunityRole?.Role is null ? "null" : userCommunityRole.Role);
    }


    public async Task<IActionResult> SubscribeUserToCommunity(Guid communityId)
    {
        var userId = await _jwtService.GetUserGuidFromTokenAsync();
        await CheckIsUserCommunityMember(userId, communityId);

        var community = await GetCommunity(communityId);
        AddSubscriberToCommunity(community, userId, communityId);
        return new OkResult();
    }


    public async Task<IActionResult> UnsubscribeUserToCommunity(Guid id)
    {
        var userId = await _jwtService.GetUserGuidFromTokenAsync();
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
        if (userCommunityRole.Role == CommunityRole.Administrator)
            throw new UserCommunityRoleNotFoundException("User is administrator of this community");
        community.SubscribersCount--;
        _db.UserCommunityRoles.Remove(userCommunityRole);
        await _db.SaveChangesAsync();
    }

    private static List<CommunityDto> ConvertCommunitiesToCommunityResponseListAsync(
        IEnumerable<Community> communities)
    {
        return communities.Select(community => new CommunityDto(community.Id, community.CreateTime, community.Name,
                community.Description, community.IsClosed, community.SubscribersCount))
            .ToList();
    }

    private static CommunityFullDto ConvertCommunityToCommunityFullResponse(Community community,
        IEnumerable<UserDto> administrators)
    {
        return new CommunityFullDto
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


    private Task<List<Guid>> GetCommunityAdminIds(Guid communityId)
    {
        return _db.UserCommunityRoles
            .Where(x => x.CommunityId == communityId && x.Role == CommunityRole.Administrator)
            .Select(x => x.UserId)
            .ToListAsync();
    }

    public async Task<IActionResult> PostCommunityPost(Guid communityId, CreatePostDto createPostDto)
    {
        var userId = await _jwtService.GetUserGuidFromTokenAsync();
        await CheckIsUserCommunityAdministrator(userId, communityId);
        var communityName = await GetCommunityNameWithCommunityId(communityId);

        return await _postService.CreateCommunityPost(communityId, communityName, createPostDto);
    }
}