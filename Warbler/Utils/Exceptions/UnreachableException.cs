namespace Warbler.Utils.Exceptions;

public class UnreachableException : Exception
{
    public UnreachableException()
    {
    }

    public UnreachableException(string message) : base(message)
    {
    }

    public UnreachableException(string message, Exception inner) : base(message, inner)
    {
    }
}