namespace BlogAPI.Exceptions;

public class BadImageLinkException : Exception
{
    public BadImageLinkException(string message) : base(message)
    {
    }
}