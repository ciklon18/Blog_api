using BlogAPI.Models.Response;

namespace BlogAPI.services.Interfaces;

public interface IAuthorService
{
    Task<List<AuthorResponse>> GetAuthorList();
}