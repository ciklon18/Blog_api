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

public partial class PostService : IPostService
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PostService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }


    public async Task<PostPagedListResponse> GetPosts(List<Guid> tagIds, string? authorName, int minReadingTime,
        int maxReadingTime, PostSorting? sort,
        bool isOnlyMyCommunities, int page, int pageSize)
    {
        var authors = await GetAuthorsByAuthorName(authorName);
        CheckIsPaginationValid(page, pageSize);
        var posts = _db.Posts.Where(x => authors.Contains(x.AuthorId) && x.CommunityId == Guid.Empty);
        posts = GetFilteredAndSortedPostsWithMinutes(posts, tagIds, sort, page, pageSize, minReadingTime,
            maxReadingTime);
        return await ConvertPostsToPostPagedListResponse(posts, page, pageSize);
    }

    private IQueryable<Post> GetFilteredAndSortedPostsWithMinutes(IQueryable<Post> posts, List<Guid> tagIds,
        PostSorting? sort, int page, int pageSize, int minReadingTime, int maxReadingTime)
    {
        var postWithoutMinutes = GetFilteredAndSortedPosts(posts, tagIds, sort, page, pageSize);
        return postWithoutMinutes.Where(x => x.ReadingTime >= minReadingTime && x.ReadingTime <= maxReadingTime);
    }

    private static void CheckIsPaginationValid(int page, int pageSize)
    {
        if (page < 1) throw new InvalidPaginationException("Invalid value for attribute page");
        if (pageSize < 1) throw new InvalidPaginationException("Invalid value for attribute pageSize");
    }

    private async Task<List<Guid>> GetAuthorsByAuthorName(string? authorName)
    {
        if (string.IsNullOrEmpty(authorName)) return await _db.Users.Select(x => x.Id).ToListAsync();
        return await _db.Users.Where(x => x.FullName.Contains(authorName)).Select(x => x.Id).ToListAsync();
    }


    public async Task<IActionResult> CreateUserPost(PostRequest postRequest)
    {
        var post = await GetConvertedPostFromPostRequest(postRequest, Guid.Empty, string.Empty);
        await _db.Posts.AddAsync(post);
        await _db.SaveChangesAsync();
        return new OkResult();
    }

    public async Task<IActionResult> CreateCommunityPost(Guid communityId, string? communityName,
        PostRequest postRequest)
    {
        var post = await GetConvertedPostFromPostRequest(postRequest, communityId, communityName);
        await _db.Posts.AddAsync(post);
        await _db.SaveChangesAsync();
        return new OkResult();
    }


    public async Task<PostResponse> GetPostById(Guid postId)
    {
        var post = await GetPost(postId);
        var userId = await GetUserGuidFromToken();
        var isLikeExist = await _db.Likes.FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId);
        post.HasLike = isLikeExist != null;
        var comments = await GetComments(postId);
        
        var postTags = await _db.PostTags.Where(x => x.PostId == postId).Select(x => x.TagId).ToListAsync();
        var tags =  ConvertPostTagsToTags(postTags);
        return ConvertPostToPostResponse(post, comments, tags); 
    }

    private async Task<List<CommentResponse>> GetComments(Guid postId)
    {
        var comments =  await _db.Comments.Where(comment => comment.PostId == postId).ToListAsync();
        return comments.Select(comment => new CommentResponse
            {
                Author = comment.Author,
                AuthorId = comment.AuthorId,
                CreateTime = comment.CreateTime.ToUniversalTime(),
                Content = comment.Content,
                DeleteDate = comment.DeleteDate,
                Id = comment.Id,
                ModifiedDate = comment.ModifiedDate,
                SubComments = comment.SubComments
            })
            .ToList();
    }

    private static PostResponse ConvertPostToPostResponse(Post post, List<CommentResponse> comments, List<TagResponse> tags)
    {
        return new PostResponse(
            post.Id,
            post.CreateTime,
            post.Title,
            post.Description,
            post.ReadingTime,
            post.Image,
            post.AuthorId,
            post.Author,
            post.CommunityId,
            post.CommunityName,
            post.AddressId,
            post.Likes,
            post.HasLike,
            post.CommentsCount,
            tags,
            comments
        );
        
    }

    private List<TagResponse> ConvertPostTagsToTags(List<Guid> toList)
    {
        var tags = new List<TagResponse>();
        foreach (var tagGuid in toList)
        {
            var tag = _db.Tags.FirstOrDefault(x => x.Id == tagGuid);
            if (tag != null)
                tags.Add(new TagResponse
                {
                    CreateTime = tag.CreateTime,
                    Id = tag.Id,
                    Name = tag.Name
                });
        }

        return tags;
    }

    public async Task<IActionResult> LikePost(Guid postId)
    {
        var post = await GetPost(postId);
        var userId = await GetUserGuidFromToken();
        var isLikeExist = await CheckIsUserLikeExist(userId, postId);
        if (isLikeExist) throw new LikeAlreadyExistException("Like already exists");
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null) throw new UserNotFoundException("User not found");
        post.Likes++;
        await _db.Likes.AddAsync(new Like
        {
            PostId = postId,
            UserId = userId
        });
        await _db.SaveChangesAsync();
        return new OkResult();
    }
    public async Task<IActionResult> UnlikePost(Guid postId)
    {
        var post = await GetPost(postId);
        var userId = await GetUserGuidFromToken();
        var isUserLikeExist = await CheckIsUserLikeExist(userId, postId);
        if (!isUserLikeExist) throw new LikeNotFoundException("Like not found");
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null) throw new UserNotFoundException("User not found");
        if (post.Likes == 0) throw new LikeNotFoundException("Like not found");
        post.Likes--;
        var like = await _db.Likes.FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId);
        if (like == null) throw new LikeNotFoundException("Like not found");
        _db.Likes.Remove(like);
        await _db.SaveChangesAsync();
        return new OkResult();
    }

    private async Task<bool> CheckIsUserLikeExist(Guid userId, Guid postId)
    {
        var isUserLikeExist = await _db.Likes.FirstOrDefaultAsync(x => x.UserId == userId && x.PostId == postId);
        return isUserLikeExist != null;
    }


    private async Task<Post> GetPost(Guid postId)
    {
        var post = await _db.Posts.FirstOrDefaultAsync(x => x.Id == postId);
        if (post == null) throw new PostNotFoundException("Post not found");
        return post;
    }




    private async Task<string?> GetUserNameWithUserId(Guid userId)
    {
        return await _db.Users
            .Where(x => x.Id == userId)
            .Select(x => x.FullName)
            .FirstOrDefaultAsync();
    }

    private async Task<Post> GetConvertedPostFromPostRequest(PostRequest postRequest, Guid communityId,
        string? communityName)
    {
        var userId = await GetUserGuidFromToken();

        await CheckAreTagsExist(postRequest.Tags);
        await CheckIsAddressExist(postRequest.AddressId);
        CheckIsImageValid(postRequest.Image);

        var userName = await GetUserNameWithUserId(userId);
        var postGuid = Guid.NewGuid();
        var postTags = ConvertTagsToPostTags(postRequest.Tags, postGuid);

        return ConvertPostRequestToPost(postRequest, userId, userName, communityId, communityName, likes: 0,
            hasLike: false, commentsCount: 0, postTags, postGuid);
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

    public async Task<PostPagedListResponse> ConvertPostsToPostPagedListResponse(IQueryable<Post> posts,
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

    public IQueryable<Post> GetFilteredAndSortedCommunityPosts(IQueryable<Post> posts, List<Guid> tagIds,
        PostSorting sort, int page, int pageSize)
    {
        return GetFilteredAndSortedPosts(posts, tagIds, sort, page, pageSize);
    }


    private static IQueryable<Post> GetFilteredAndSortedPosts(IQueryable<Post> posts, List<Guid> tagIds,
        PostSorting? sort, int page, int pageSize)
    {
        var updatedPosts = GetSortedPosts(posts, sort);
        updatedPosts = tagIds.Count != 0
            ? updatedPosts.Where(x => x.PostTags.Any(tag => tagIds.Contains(tag.TagId)))
            : updatedPosts;
        updatedPosts = updatedPosts.Skip((page - 1) * pageSize).Take(pageSize);
        return updatedPosts;
    }

    private static IQueryable<Post> GetSortedPosts(IQueryable<Post> posts, PostSorting? sort)
    {
        if (sort == null) return posts;
        return sort switch
        {
            PostSorting.CreateDesc => posts.OrderByDescending(x => x.CreateTime),
            PostSorting.LikeDesc => posts.OrderByDescending(x => x.Likes),
            PostSorting.CreateAsc => posts.OrderBy(x => x.CreateTime),
            PostSorting.LikeAsc => posts.OrderBy(x => x.Likes),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private async Task<Guid> GetUserGuidFromToken()
    {
        var userEmail = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
        if (userEmail == null) throw new UserNotFoundException("User not found");
        await CheckIsRefreshTokenValid(userEmail);
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == userEmail);
        if (user == null) throw new UserNotFoundException("User not found");
        return user.Id;
    }

    private async Task CheckIsRefreshTokenValid(string email)
    {
        var isEmailUsed = await _db.RefreshTokens.AnyAsync(u => u.Email == email);
        if (!isEmailUsed) throw new UnauthorizedException("Refresh token is not valid");
    }

    [GeneratedRegex(pattern: EntityConstants.ImageUrlRegex)]
    private static partial Regex ImageLinkRegex();
}