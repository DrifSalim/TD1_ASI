using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface INoteRepository : IRepository<Note>
{
    Task AddNoteAsync(Note note);
    Task<Parcours> AddNoteAsync(Etudiant e, Ue ue, float note);
    Task<Parcours> AddNoteAsync(long idEtudiant, long idUe, float note);
}