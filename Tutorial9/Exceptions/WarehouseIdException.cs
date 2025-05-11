namespace Tutorial9.Exceptions;

public class WarehouseIdException : Exception
{
    public WarehouseIdException()
    {
    }

    public WarehouseIdException(string? message) : base(message)
    {
    }

    public WarehouseIdException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}