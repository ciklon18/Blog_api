using BlogAPI.Models.Response;

namespace BlogAPI.services.Interfaces;

public interface ITagService
{
    Task<List<TagResponse>> GetTags();
}