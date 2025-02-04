namespace UniversiteDomain.Exceptions.NotesExceptions;

public class NoteInvalideException : Exception
{
    public NoteInvalideException() : base() { }
    public NoteInvalideException(string message) : base(message) { }

    public NoteInvalideException(string message, Exception inner) : base(message, inner)
    {
    }
}