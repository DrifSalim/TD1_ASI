namespace UniversiteDomain.Exceptions.NotesExceptions;

public class NoteExistanteException : Exception
{
    public NoteExistanteException() : base() { }
    public NoteExistanteException(string message) : base(message) { }

    public NoteExistanteException(string message, Exception inner) : base(message, inner)
    {
    }
    
}