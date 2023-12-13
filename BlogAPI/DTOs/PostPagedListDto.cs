using BlogAPI.Models;

namespace BlogAPI.DTOs;

public record PostPagedListDto(List<PostDto> Posts, PageInfoModel Pagination)
{
    public PostPagedListDto() : this(new List<PostDto>(), new PageInfoModel())
    {
    }
}