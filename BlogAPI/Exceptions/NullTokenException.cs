﻿namespace BlogAPI.Exceptions;

public class NullTokenException : Exception
{
    public NullTokenException(string message) : base(message)
    {
    }
}