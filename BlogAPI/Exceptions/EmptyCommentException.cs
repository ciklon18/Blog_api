namespace BlogAPI.Exceptions;

public class EmptyCommentException : Exception
{
    public EmptyCommentException(string message) : base(message)
    {
    }
}