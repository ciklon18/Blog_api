namespace BlogAPI.Exceptions;

public class LikeNotFoundException : Exception
{
    public LikeNotFoundException(string message) : base(message)
    {
    }
}