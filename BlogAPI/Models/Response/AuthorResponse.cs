using BlogAPI.Enums;

namespace BlogAPI.Models.Response;

public record AuthorResponse(string FullName, DateTime BirthDate, Gender Gender, int Posts, int Likes, DateTime Created)
{
    public AuthorResponse() : this(string.Empty, DateTime.Now, Enums.Gender.Male, 0, 0, DateTime.Now)
    {
        
    }
}