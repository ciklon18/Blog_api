using BlogAPI.DTOs;
using BlogAPI.Models.Response;

namespace BlogAPI.services.Interfaces;

public interface IAuthorService
{
    Task<List<AuthorDto>> GetAuthorList();
}