using BlogAPI.Models.Response;
using BlogAPI.services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.controllers;

[ApiController]
[Route("api/[controller]")]
public class TagController
{
    private readonly ITagService _tagService;
    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }
    [HttpGet]
    public async Task<List<TagResponse>> GetTags()
    {
        return await _tagService.GetTags();
    }
}