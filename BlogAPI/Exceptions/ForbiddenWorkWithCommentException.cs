namespace BlogAPI.Exceptions;

public class ForbiddenWorkWithCommentException : Exception
{
    public ForbiddenWorkWithCommentException(string message) : base(message)
    {
    }
}