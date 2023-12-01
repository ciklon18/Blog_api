namespace BlogAPI.Exceptions;

public class AddressElementNotFound : Exception
{
    public AddressElementNotFound(string message) : base(message)
    {
    }
}