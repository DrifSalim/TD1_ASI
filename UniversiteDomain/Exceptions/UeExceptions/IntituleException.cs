namespace UniversiteDomain.Exceptions.UeExceptions;

public class IntituleException : Exception
{
    public IntituleException() : base() { }
    public IntituleException(string message) : base(message) { }

    public IntituleException(string message, Exception inner) : base(message, inner)
    {
    }
}