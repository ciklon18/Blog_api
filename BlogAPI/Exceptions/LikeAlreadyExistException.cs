namespace BlogAPI.Exceptions;

public class LikeAlreadyExistException : Exception
{
    public LikeAlreadyExistException(string message) : base(message)
    {
    }
}