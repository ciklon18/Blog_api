using BlogAPI.DTOs;
using BlogAPI.services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.controllers;

[ApiController]
[Route("api/[controller]")]
public class TagController : ControllerBase
{
    private readonly ITagService _tagService;
    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }
    [HttpGet]
    public Task<List<TagDto>> GetTags()
    {
        return _tagService.GetTags();
    }
}