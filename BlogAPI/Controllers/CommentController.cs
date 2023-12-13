using BlogAPI.DTOs;
using BlogAPI.Models.Request;
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
    public Task<List<CommentDto>> GetCommentTree([FromRoute] Guid id)
    {
        return _commentService.GetCommentTree(id);
    }
    
    [HttpPost("{id}/comment"), Authorize]
    public Task<IActionResult> CreateComment([FromRoute] Guid id, CreateCommentDto dto)
    {
        return _commentService.CreateComment(id, dto);
    }
    
    [HttpPut("{id}"), Authorize]
    public Task<IActionResult> EditComment([FromRoute] Guid id, UpdateCommentDto dto)
    {
        return _commentService.EditComment(id, dto);
    }
    
    [HttpDelete("{id}"), Authorize]
    public Task<IActionResult> DeleteComment([FromRoute] Guid id)
    {
           return _commentService.DeleteComment(id);
    }
}