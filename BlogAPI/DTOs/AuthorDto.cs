using BlogAPI.Enums;

namespace BlogAPI.DTOs;

public record AuthorDto(string FullName, DateTime BirthDate, Gender Gender, int Posts, int Likes, DateTime Created)
{
    public AuthorDto() : this(string.Empty, DateTime.Now, Enums.Gender.Male, 0, 0, DateTime.Now)
    {
        
    }
}