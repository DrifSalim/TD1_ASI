using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface INoteRepository : IRepository<Note>
{
    Task AddNoteAsync(Note note);
    Task AddNoteAsync(Etudiant e, Ue ue, float note);
    Task AddNoteAsync(long idEtudiant, long idUe, float note);
}