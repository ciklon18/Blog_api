using BlogAPI.Models.Response;
using BlogAPI.services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorController : ControllerBase
{
    private readonly IAuthorService _authorService;

    public AuthorController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    [HttpGet("/api/author/list")]
    public async Task<List<AuthorResponse>> GetAuthorList()
    {
        return await _authorService.GetAuthorList();
    }
}