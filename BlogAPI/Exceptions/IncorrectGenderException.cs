namespace BlogAPI.Exceptions;

public class IncorrectGenderException : Exception
{
    public IncorrectGenderException(string message) : base(message)
    {
    }
}