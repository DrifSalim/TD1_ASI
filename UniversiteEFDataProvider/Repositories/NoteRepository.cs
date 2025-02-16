using Microsoft.EntityFrameworkCore;
using UniversiteDomain.Entities;
using UniversiteDomain.DataAdapters;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class NoteRepository(UniversiteDbContext context) : Repository<Note>(context), INoteRepository
{
    public async Task AddNoteAsync(Note note)
    {
        ArgumentNullException.ThrowIfNull(note);
        await Context.Notes.AddAsync(note);
        await Context.SaveChangesAsync();
    }

    /*public async Task AddNoteAsync(Etudiant etudiant, Ue ue, float note)
    {
        ArgumentNullException.ThrowIfNull(etudiant);
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(note);

        var nouvelleNote = new Note
        {
            Etudiant = etudiant,
            Ue = ue,
            Valeur = note
        };

        await AddNoteAsync(nouvelleNote);
    }*/

    public async Task AddNoteAsync(long idEtudiant, long idUe, float note)
    {
        ArgumentNullException.ThrowIfNull(idEtudiant);
        ArgumentNullException.ThrowIfNull(idUe);
        ArgumentNullException.ThrowIfNull(note);

        Etudiant? etudiant = await Context.Etudiants.FindAsync(idEtudiant);
        Ue? ue = await Context.Ues.FindAsync(idUe);

        if (etudiant == null || ue == null)
            throw new ArgumentException("Étudiant ou UE introuvable");

        await AddNoteAsync(etudiant, ue, note);
        
    }
    // Dans NoteRepository
    public async Task AddNoteAsync(Etudiant etudiant, Ue ue, float note)
    {
        // Recherche d'une note existante
        var noteExistante = await context.Notes
            .FirstOrDefaultAsync(n => n.EtudiantId == etudiant.Id && n.UeId == ue.Id);

        if (noteExistante != null)
        {
            // Mise à jour de la note existante
            noteExistante.Valeur = note;
        }
        else
        {
            // Création d'une nouvelle note
            var nouvelleNote = new Note
            {
                EtudiantId = etudiant.Id,
                UeId = ue.Id,
                Valeur = note
            };
            await context.Notes.AddAsync(nouvelleNote);
        }

        await context.SaveChangesAsync();
    }
}