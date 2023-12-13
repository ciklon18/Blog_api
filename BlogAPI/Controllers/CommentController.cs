using BlogAPI.DTOs;
using BlogAPI.Models.Request;
using BlogAPI.Models.Response;
using BlogAPI.services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;
    
    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }
 
    [HttpGet("{id}/tree")]
    public async Task<List<CommentDto>> GetCommentTree([FromRoute] Guid id)
    {
        return await _commentService.GetCommentTree(id);
    }
    
    [HttpPost("{id}/comment"), Authorize]
    public async Task<IActionResult> CreateComment([FromRoute] Guid id, CreateCommentRequest request)
    {
        return await _commentService.CreateComment(id, request);
    }
    
    [HttpPut("{id}"), Authorize]
    public async Task<IActionResult> EditComment([FromRoute] Guid id, UpdateCommentRequest request)
    {
        return await _commentService.EditComment(id, request);
    }
    
    [HttpDelete("{id}"), Authorize]
    public async Task<IActionResult> DeleteComment([FromRoute] Guid id)
    {
           return await _commentService.DeleteComment(id);
    }
}