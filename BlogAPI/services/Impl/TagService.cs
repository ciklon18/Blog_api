using BlogAPI.Data;
using BlogAPI.Models.Response;
using BlogAPI.services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.services.Impl;

public class TagService : ITagService
{
    private readonly ApplicationDbContext _db;
    
    public TagService(ApplicationDbContext db)
    {
        _db = db;
    }
    
    public async Task<List<TagDto>> GetTags()
    {
        return await _db.Tags.Select(x => new TagDto(x.Id, x.CreateTime, x.Name)).ToListAsync();
    }

}