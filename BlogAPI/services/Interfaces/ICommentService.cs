using BlogAPI.DTOs;
using BlogAPI.Models.Request;
using BlogAPI.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.services.Interfaces;

public interface ICommentService
{
    Task<List<CommentDto>> GetCommentTree(Guid commentId);
    Task<IActionResult> CreateComment(Guid postId, CreateCommentDto dto);
    Task<IActionResult> EditComment(Guid commentId, UpdateCommentDto dto);
    Task<IActionResult> DeleteComment(Guid commentId);
}