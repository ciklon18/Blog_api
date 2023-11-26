namespace BlogAPI.Exceptions;

public class InvalidRefreshToken : Exception
{
    public InvalidRefreshToken(string message) : base(message)
    {
    }
}