using BlogAPI.Models.Response;

namespace BlogAPI.services.Interfaces;

public interface ITagService
{
    Task<List<TagDto>> GetTags();
}