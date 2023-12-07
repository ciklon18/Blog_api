using System.Security.Claims;
using System.Text.RegularExpressions;
using BlogAPI.Configurations;
using BlogAPI.Data;
using BlogAPI.Entities;
using BlogAPI.Enums;
using BlogAPI.Exceptions;
using BlogAPI.Models.Request;
using BlogAPI.Models.Response;
using BlogAPI.services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.services.Impl;

public partial class CommunityService : ICommunityService
{
    private readonly ApplicationDbContext _db;
    private readonly IJwtService _jwtService;

    public CommunityService(ApplicationDbContext db, IJwtService jwtService)
    {
        _db = db;
        _jwtService = jwtService;
    }

    public async Task<List<CommunityResponse>> GetCommunityList()
    {
        var result = await _db.Communities.ToListAsync();
        return ConvertCommunitiesToCommunityResponseListAsync(result);
    }


    public async Task<List<CommunityUserResponse>> GetMyCommunityList()
    {
        var userId = await _jwtService.GetUserGuidFromToken();
        var result = _db.UserCommunityRoles.Where(x => x.UserId == userId).ToList();
        return result.Select(x => new CommunityUserResponse(x.UserId, x.CommunityId, x.Role.ToString()))
            .ToList();
    }


    public async Task<CommunityFullResponse> GetCommunityInfo(Guid id)
    {
        var community = await _db.Communities.FirstOrDefaultAsync(community => community.Id == id);
        if (community == null) throw new CommunityNotFoundException("Community not found");
        var adminIds = await GetCommunityAdminIds(id);
        var admins = await _db.Users.Where(x => adminIds.Contains(x.Id)).ToListAsync();

        return ConvertCommunityToCommunityFullResponse(community, admins);
    }


    public async Task<PostPagedListResponse> GetCommunityPosts(Guid communityId, List<Guid> tagIds, PostSorting sort,
        int page, int pageSize)
    {
        await CheckIsThereCommunity(communityId);
        CheckIsPaginationValid(page, pageSize);
        var userId = await _jwtService.GetUserGuidFromToken();
        if (await IsCommunityClosed(communityId))
            await CheckIsUserSubscribedToCommunity(communityId, userId);
        var posts = _db.Posts.Where(x => x.CommunityId == communityId);
        posts = GetFilteredAndSortedPosts(posts, tagIds, sort, page, pageSize);
        return await ConvertPostsToPostPagedListResponse(posts, page, pageSize);
    }

    private static IQueryable<Post> GetFilteredAndSortedPosts(IQueryable<Post> posts, ICollection<Guid> tagIds,
        PostSorting sort, int page, int pageSize)
    {
        posts = GetSortedPosts(posts, sort);
        posts = tagIds.Count != 0 ? posts.Where(x => x.PostTags.Any(tag => tagIds.Contains(tag.TagId))) : posts;
        posts = posts.Skip((page - 1) * pageSize).Take(pageSize);
        return posts;
    }

    private static void CheckIsPaginationValid(int page, int pageSize)
    {
        if (page < 1) throw new InvalidPaginationException("Invalid value for attribute page");
        if (pageSize < 1) throw new InvalidPaginationException("Invalid value for attribute pageSize");
    }

    private async Task<bool> IsCommunityClosed(Guid communityId)
    {
        return await _db.Communities.Where(x => x.Id == communityId).Select(x => x.IsClosed).FirstOrDefaultAsync();
    }

    private async Task CheckIsUserSubscribedToCommunity(Guid communityId, Guid userId)
    {
        var userCommunityRole =
            await _db.UserCommunityRoles.FirstOrDefaultAsync(x => x.UserId == userId && x.CommunityId == communityId);
        if (userCommunityRole == null)
            throw new ForbiddenAccessToClosedCommunityException(
                $"Access to closed community with id={communityId} is forbidden");
    }


    public async Task<IActionResult> PostCommunityPost(Guid communityId, PostRequest postRequest)
    {
        var userId = await _jwtService.GetUserGuidFromToken();
        await CheckIsUserCommunityAdministrator(userId, communityId);

        await CheckAreTagsExist(postRequest.Tags);
        await CheckIsAddressExist(postRequest.AddressId);
        CheckIsImageValid(postRequest.Image);

        var communityName = await GetCommunityNameWithCommunityId(communityId);
        var userName = await GetUserNameWithUserId(userId);
        var postGuid = Guid.NewGuid();
        var postTags = ConvertTagsToPostTags(postRequest.Tags, postGuid);

        var post = ConvertPostRequestToPost(postRequest, userId, userName, communityId, communityName, likes: 0,
            hasLike: false, commentsCount: 0, postTags, postGuid);

        await _db.Posts.AddAsync(post);
        await _db.SaveChangesAsync();
        return new OkResult();
    }

    private static void CheckIsImageValid(string? image)
    {
        if (!string.IsNullOrEmpty(image) && !ImageLinkRegex().IsMatch(image))
        {
            throw new BadImageLinkException("Image link is not valid");
        }
    }


    private async Task CheckAreTagsExist(ICollection<Guid> tags)
    {
        var tagsFromDb = await _db.Tags.Where(x => tags.Contains(x.Id)).ToListAsync();
        var areTagsExist = tags.All(tag => tagsFromDb.Any(tagFromDb => tagFromDb.Id == tag));
        if (!areTagsExist) throw new TagNotFoundException("Tag not found");
    }

    private async Task CheckIsAddressExist(Guid? addressId)
    {
        if (addressId == Guid.Empty) return;
        var address = await _db.Addresses.FirstOrDefaultAsync(x => x.ObjectGuid == addressId);
        if (address != null) return;
        var housesAddress = await _db.HousesAddresses.FirstOrDefaultAsync(x => x.ObjectGuid == addressId);
        if (housesAddress == null) throw new AddressElementNotFound("Address not found");
    }


    private static List<PostTag> ConvertTagsToPostTags(IEnumerable<Guid> tags, Guid postId)
    {
        return tags.Select(tagGuid => new PostTag
        {
            TagId = tagGuid,
            PostId = postId
        }).ToList();
    }

    private async Task CheckIsUserCommunityAdministrator(Guid userId, Guid communityId)
    {
        var userCommunityRole =
            await _db.UserCommunityRoles.FirstOrDefaultAsync(x => x.UserId == userId && x.CommunityId == communityId);
        if (userCommunityRole?.Role != CommunityRole.Administrator)
            throw new UserCommunityRoleNotFoundException("User does not have administrator role in this community");
    }

    private async Task<string?> GetUserNameWithUserId(Guid userId)
    {
        return await _db.Users
            .Where(x => x.Id == userId)
            .Select(x => x.FullName)
            .FirstOrDefaultAsync();
    }

    private async Task<string?> GetCommunityNameWithCommunityId(Guid communityId)
    {
        return await _db.Communities
            .Where(x => x.Id == communityId)
            .Select(x => x.Name)
            .FirstOrDefaultAsync();
    }

    public async Task<OkObjectResult> GetCommunityUserRole(Guid id)
    {
        await CheckIsThereCommunity(id);
        var userId = await _jwtService.GetUserGuidFromToken();
        var userCommunityRole =
            await _db.UserCommunityRoles.FirstOrDefaultAsync(x => x.UserId == userId && x.CommunityId == id);
        return new OkObjectResult(userCommunityRole?.Role is null ? "null" : userCommunityRole.Role);
    }


    public async Task<IActionResult> SubscribeUserToCommunity(Guid communityId)
    {
        var userId = await _jwtService.GetUserGuidFromToken();
        await CheckIsUserCommunityMember(userId, communityId);

        var community = await GetCommunity(communityId);
        AddSubscriberToCommunity(community, userId, communityId);
        return new OkResult();
    }


    public async Task<IActionResult> UnsubscribeUserToCommunity(Guid id)
    {
        var userId = await _jwtService.GetUserGuidFromToken();
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

    private static async Task<PostPagedListResponse> ConvertPostsToPostPagedListResponse(IQueryable<Post> posts,
        int page, int pageSize)
    {
        var count = await posts.CountAsync();
        if (page > count / pageSize + 1) throw new InvalidPaginationException("Invalid value for attribute page");

        return new PostPagedListResponse
        {
            Posts = await posts.ToListAsync(),
            Pagination = new PageInfoResponse
            {
                Size = pageSize,
                Count = count,
                Current = page
            }
        };
    }


    private static IQueryable<Post> GetSortedPosts(IQueryable<Post> posts, PostSorting sort)
    {
        return sort switch
        {
            PostSorting.CreateDesc => posts.OrderByDescending(x => x.CreateTime),
            PostSorting.LikeDesc => posts.OrderByDescending(x => x.Likes),
            PostSorting.CreateAsc => posts.OrderBy(x => x.CreateTime),
            PostSorting.LikeAsc => posts.OrderBy(x => x.Likes),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private async Task DeleteSubscriberFromCommunity(Community community, UserCommunityRole userCommunityRole)
    {
        if (userCommunityRole.Role == CommunityRole.Administrator)
            throw new UserCommunityRoleNotFoundException("User is administrator of this community");
        community.SubscribersCount--;
        _db.UserCommunityRoles.Remove(userCommunityRole);
        await _db.SaveChangesAsync();
    }

    private static List<CommunityResponse> ConvertCommunitiesToCommunityResponseListAsync(
        IEnumerable<Community> communities)
    {
        return communities.Select(community => new CommunityResponse(community.Id, community.CreateTime, community.Name,
                community.Description, community.IsClosed, community.SubscribersCount))
            .ToList();
    }

    private static CommunityFullResponse ConvertCommunityToCommunityFullResponse(Community community,
        List<User> administrators)
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



    private async Task<List<Guid>> GetCommunityAdminIds(Guid communityId)
    {
        return await _db.UserCommunityRoles
            .Where(x => x.CommunityId == communityId && x.Role == CommunityRole.Administrator)
            .Select(x => x.UserId)
            .ToListAsync();
    }

    private static Post ConvertPostRequestToPost(PostRequest postRequest, Guid userId, string? userName,
        Guid communityId,
        string? communityName, int likes, bool hasLike, int commentsCount, ICollection<PostTag>? tags, Guid postGuid)
    {
        return new Post
        {
            Id = postGuid,
            CreateTime = DateTime.Now.ToUniversalTime(),
            Title = postRequest.Title,
            Description = postRequest.Description,
            ReadingTime = postRequest.ReadingTime,
            Image = postRequest.Image,
            AuthorId = userId,
            Author = userName ?? "",
            CommunityId = communityId,
            CommunityName = communityName ?? "",
            AddressId = postRequest.AddressId ?? Guid.Empty,
            Likes = likes,
            HasLike = hasLike,
            CommentsCount = commentsCount,
            PostTags = tags ?? new List<PostTag>()
        };
    }

    [GeneratedRegex(pattern: EntityConstants.ImageUrlRegex)]
    private static partial Regex ImageLinkRegex();
}