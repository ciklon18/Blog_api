namespace BlogAPI.Exceptions;

public class UserCommunityRoleAlreadyExistsException : Exception
{
    public UserCommunityRoleAlreadyExistsException(string message) : base(message)
    {
    }
}