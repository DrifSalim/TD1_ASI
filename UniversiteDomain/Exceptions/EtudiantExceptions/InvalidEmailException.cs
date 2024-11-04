namespace UniversiteDomain.Exceptions.EtudiantExceptions;

public class InvalidEmailException : Exception
{
    public InvalidEmailException() : base() { }
    public InvalidEmailException(string message) : base(message) { }
    public InvalidEmailException(string message, Exception inner) : base(message, inner) { }
}