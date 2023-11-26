namespace BlogAPI.Configurations;

public static class EntityConstants
{
    public const string PhoneNumberRegex = @"^(\+7|8)[0-9]{10}$";
    public const string IncorrectPhoneNumberError = "Invalid phone number format. It must be +7xxxxxxxxxx or 8xxxxxxxxxx";

    public const string ShortPasswordError = "Too short password. Password must be 6 or more  characters.";

    public const string FullNameRegex = @"^[a-zA-Z\s]*$";
    public const string WrongSymbolInFullNameError = "Name can only contain letters and spaces.";
    public const string IncorrectEmailError = "Invalid email format. It must be user@example.com";
    public const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
}