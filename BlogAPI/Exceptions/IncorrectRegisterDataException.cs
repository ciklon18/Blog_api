namespace BlogAPI.Exceptions;

public class IncorrectRegisterDataException : Exception
{
    public IncorrectRegisterDataException(string message) : base(message)
    {
    }
}