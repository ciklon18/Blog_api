using BlogAPI.Data;
using BlogAPI.DTOs;
using BlogAPI.Entities;
using BlogAPI.Enums;
using BlogAPI.services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.services.Impl;

public class AuthorService : IAuthorService
{
    private readonly ApplicationDbContext _db;

    public AuthorService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<AuthorDto>> GetAuthorList()
    {
        var authors = await GetAuthorsWithDetails();
        var authorIds = authors.Select(author => author.UserId).ToList();
        
        var communities = await GetCommunities(authorIds);
        var communitiesIds = communities.Select(community => community.Id).ToList();
        var posts = await GetPostsForCommunities(communitiesIds);
        var (likesHashMap, postsHashMap) = GenerateHashMaps(posts);
        
        return authors
            .Select(author => MapToAuthorResponse(author, likesHashMap, postsHashMap))
            .Where(authorDto => authorDto.Posts != 0)
            .ToList();
    }


    private async Task<IReadOnlyCollection<Community>> GetCommunities(IEnumerable<Guid> authorIds)
    {
        return await _db.UserCommunityRoles
            .Where(ucr => authorIds.Contains(ucr.UserId) && ucr.Role == CommunityRole.Administrator)
            .Select(ucr => ucr.Community)
            .ToListAsync();
    }


    private async Task<List<UserCommunityRole>> GetAuthorsWithDetails()
    {
        var users = await _db.UserCommunityRoles
            .Where(ucr => ucr.Role == CommunityRole.Administrator)
            .Include(ucr => ucr.User)
            .ToListAsync();

        var usersIds = users.Select(user => user.UserId).ToList();
        var usersWithoutDetails = await _db.Users
            .Where(user => usersIds.Contains(user.Id))
            .ToListAsync();

        foreach (var user in usersWithoutDetails.Where(user => users.All(u => u.UserId != user.Id)))
        {
            users.Add(new UserCommunityRole
            {
                UserId = user.Id,
                User = user
            });
        }

        return users;
    }

    private async Task<List<Post>> GetPostsForCommunities(IReadOnlyCollection<Guid> communitiesIds)
    {
        return await _db.Posts
            .Where(post => communitiesIds.Contains(post.CommunityId ?? Guid.Empty))
            .ToListAsync();
    }


    private static (Dictionary<Guid, int> likesHashMap, Dictionary<Guid, int> postsHashMap) GenerateHashMaps(
        IEnumerable<Post> posts)
    {
        var likesHashMap = new Dictionary<Guid, int>();
        var postsHashMap = new Dictionary<Guid, int>();

        foreach (var post in posts.Where(post => post.AuthorId != Guid.Empty))
        {
            likesHashMap[post.AuthorId] = likesHashMap.GetValueOrDefault(post.AuthorId, 0) + post.Likes;
            postsHashMap[post.AuthorId] = postsHashMap.GetValueOrDefault(post.AuthorId, 0) + 1;
        }

        return (likesHashMap, postsHashMap);
    }

    private static AuthorDto MapToAuthorResponse(UserCommunityRole author,
        IReadOnlyDictionary<Guid, int> likesHashMap, IReadOnlyDictionary<Guid, int> postsHashMap)
    {
        return new AuthorDto
        {
            BirthDate = author.User.BirthDate.ToUniversalTime(),
            Created = author.User.CreatedAt.ToUniversalTime(),
            FullName = author.User.FullName,
            Gender = author.User.Gender,
            Likes = likesHashMap.GetValueOrDefault(author.UserId, 0),
            Posts = postsHashMap.GetValueOrDefault(author.UserId, 0)
        };
    }
}