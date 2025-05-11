namespace Tutorial9.Exceptions;

public class AmountException : Exception
{
    public AmountException()
    {
    }

    public AmountException(string? message) : base(message)
    {
    }

    public AmountException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}