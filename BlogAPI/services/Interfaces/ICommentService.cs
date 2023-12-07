using BlogAPI.Models.Request;
using BlogAPI.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.services.Interfaces;

public interface ICommentService
{
    Task<List<CommentResponse>> GetCommentTree(Guid commentId);
    Task<IActionResult> CreateComment(Guid postId, CreateCommentRequest request);
    Task<IActionResult> EditComment(Guid commentId, UpdateCommentRequest request);
    Task<IActionResult> DeleteComment(Guid commentId);
}