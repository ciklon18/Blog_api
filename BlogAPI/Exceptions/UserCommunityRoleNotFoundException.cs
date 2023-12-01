namespace BlogAPI.Exceptions;

public class UserCommunityRoleNotFoundException : Exception
{
    public UserCommunityRoleNotFoundException(string message) : base(message)
    {
    }
}