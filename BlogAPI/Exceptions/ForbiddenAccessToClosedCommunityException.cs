namespace BlogAPI.Exceptions;

public class ForbiddenAccessToClosedCommunityException : Exception
{
    public ForbiddenAccessToClosedCommunityException(string message) : base(message)
    {
    }
}