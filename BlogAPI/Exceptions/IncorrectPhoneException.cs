namespace BlogAPI.Exceptions;

public class IncorrectPhoneException : Exception
{
    public IncorrectPhoneException(string message) : base(message)
    {
    }
}