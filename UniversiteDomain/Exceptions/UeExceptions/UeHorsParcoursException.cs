namespace UniversiteDomain.Exceptions.NotesExceptions;

public class UeHorsParcoursException : Exception
{
    public UeHorsParcoursException() : base() { }
    public UeHorsParcoursException(string message) : base(message) { }

    public UeHorsParcoursException(string message, Exception inner) : base(message, inner)
    {
    }
}