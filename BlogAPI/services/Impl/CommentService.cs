using BlogAPI.Data;
using BlogAPI.DTOs;
using BlogAPI.Entities;
using BlogAPI.Exceptions;
using BlogAPI.Models.Request;
using BlogAPI.Models.Response;
using BlogAPI.services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BlogAPI.services.Impl;

public class CommentService : ICommentService
{
    private readonly ApplicationDbContext _db;
    private readonly IJwtService _jwtService;

    public CommentService(ApplicationDbContext db, IJwtService jwtService)
    {
        _db = db;
        _jwtService = jwtService;
    }


    public async Task<List<CommentDto>> GetCommentTree(Guid commentId)
    {
        await CheckIsCommentExist(commentId);
        var comments = await _db.Comments
            .Where(comment => comment.ParentId == commentId && comment.DeleteDate == null)
            .ToListAsync();
        return comments.Select(ConvertCommentToResponse).ToList();
    }

    private async Task CheckIsCommentExist(Guid commentId)
    {
        var comment = await _db.Comments.FirstOrDefaultAsync(comment => comment.Id == commentId);
        if (comment is not { DeleteDate: null }) throw new CommentNotFoundException("Comment not found");
    }

    private static CommentDto ConvertCommentToResponse(Comment comment)
    {
        return new CommentDto
        {
            Id = comment.Id,
            Author = comment.Author,
            AuthorId = comment.AuthorId,
            Content = comment.DeleteDate != null ? string.Empty : comment.Content,
            CreateTime = comment.CreateTime,
            DeleteDate = comment.DeleteDate,
            ModifiedDate = comment.ModifiedDate,
            SubComments = comment.SubComments
        };
    }

    public async Task<IActionResult> CreateComment(Guid postId, CreateCommentRequest request)
    {
        CheckIsCommentEmpty(request.Content);
        if (request.ParentId != null && request.ParentId != Guid.Empty)
        {
            await CheckIsThereParentComment(request.ParentId);
            await UpdateSubCommentsCount(request.ParentId);
        }
        var userId = await _jwtService.GetUserGuidFromTokenAsync();
        var author = await GetUserDtoByGuid(userId);
        var comment = ConvertRequestToComment(request, author.FullName, author.Id, postId);
        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();
        return new OkResult();
    }


    public async Task<IActionResult> EditComment(Guid commentId, UpdateCommentRequest request)
    {
        CheckIsCommentEmpty(request.Content);
        var userId = await _jwtService.GetUserGuidFromTokenAsync();
        var comment = await _db.Comments.FirstOrDefaultAsync(x => x.Id == commentId);
        if (comment is not { DeleteDate: null }) throw new CommentNotFoundException("Comment not found");
        if (comment.AuthorId != userId)
            throw new ForbiddenWorkWithCommentException("You are not the author of this comment");
        comment.Content = request.Content;
        comment.ModifiedDate = DateTime.Now.ToUniversalTime();
        _db.Comments.Update(comment);
        await _db.SaveChangesAsync();
        return new OkResult();
    }

    private static void CheckIsCommentEmpty(string content)
    {
        if (content.IsNullOrEmpty()) throw new EmptyCommentException("The Content field is required.");
    }

    public async Task<IActionResult> DeleteComment(Guid commentId)
    {
        var userId = await _jwtService.GetUserGuidFromTokenAsync();
        var comment = await _db.Comments.FirstOrDefaultAsync(x => x.Id == commentId);
        if (comment == null) throw new CommentNotFoundException("Comment not found");
        if (comment.AuthorId != userId)
            throw new ForbiddenWorkWithCommentException("You are not the author of this comment");
        if (comment.SubComments > 0)
        {
            comment.ModifiedDate = DateTime.Now.ToUniversalTime();
            comment.DeleteDate = DateTime.Now.ToUniversalTime();
            _db.Comments.Update(comment);
            await _db.SaveChangesAsync();
            return new OkResult();
        }
        else
        {
            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();
            return new OkResult();
        }

    }

    private async Task CheckIsThereParentComment(Guid? requestParentId)
    {
        var parentComment = await _db.Comments.FirstOrDefaultAsync(x => x.Id == requestParentId);
        if (parentComment == null) throw new CommentNotFoundException("Parent comment not found");
    }


    private async Task<User> GetUserDtoByGuid(Guid userId)
    {
        var author = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (author == null) throw new UserNotFoundException("User not found");
        return author;
    }

    private async Task UpdateSubCommentsCount(Guid? commentId)
    {
        var comment = await _db.Comments.FirstOrDefaultAsync(x => x.Id == commentId);
        if (comment == null) throw new CommentNotFoundException("Comment not found");
        comment.SubComments += 1;
        _db.Comments.Update(comment);
        await _db.SaveChangesAsync();
    }

    private static Comment ConvertRequestToComment(CreateCommentRequest request, string authorName, Guid authorId, Guid postId)
    {
        return new Comment
        {
            Id = Guid.NewGuid(),
            Content = request.Content,
            Author = authorName,
            AuthorId = authorId,
            CreateTime = DateTime.Now.ToUniversalTime(),
            DeleteDate = null,
            ModifiedDate = null,
            ParentId = request.ParentId,
            PostId = postId,
            SubComments = 0,
        };
    }
}