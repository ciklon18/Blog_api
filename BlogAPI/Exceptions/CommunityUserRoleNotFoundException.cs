namespace BlogAPI.Exceptions;

public class CommunityUserRoleNotFoundException : Exception
{
    public CommunityUserRoleNotFoundException(string message) : base(message)
    {
    }
}