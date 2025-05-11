namespace Tutorial9.Exceptions;

public class ProductIdException : Exception
{
    public ProductIdException()
    {
    }

    public ProductIdException(string? message) : base(message)
    {
    }

    public ProductIdException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}