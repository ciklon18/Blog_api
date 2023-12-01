using BlogAPI.Entities;

namespace BlogAPI.Models.Response;

public record PostPagedListResponse(List<Post> Posts, PageInfoResponse Pagination)
{
    public PostPagedListResponse() : this(new List<Post>(), new PageInfoResponse())
    {
    }
}