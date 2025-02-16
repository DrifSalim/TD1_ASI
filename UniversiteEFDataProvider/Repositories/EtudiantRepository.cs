using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteEFDataProvider.Data;
using UniversiteDomain.Entities;


namespace UniversiteEFDataProvider.Repositories;

public class EtudiantRepository(UniversiteDbContext context) : Repository<Etudiant>(context), IEtudiantRepository
{
    public async Task AffecterParcoursAsync(long idEtudiant, long idParcours)
    {
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        Etudiant e = (await Context.Etudiants.FindAsync(idEtudiant))!;
        Parcours p = (await Context.Parcours.FindAsync(idParcours))!;
        e.ParcoursSuivi = p;
        await Context.SaveChangesAsync();
    }
    
    public async Task AffecterParcoursAsync(Etudiant etudiant, Parcours parcours)
    {
        await AffecterParcoursAsync(etudiant.Id, parcours.Id); 
    }
    public async Task<Etudiant?> FindEtudiantCompletAsync(long idEtudiant)
    {
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        return await Context.Etudiants
            .Include(e => e.NotesObtenues)
            .ThenInclude(n => n.Ue)
            .Include(e => e.ParcoursSuivi)  
            .FirstOrDefaultAsync(e => e.Id == idEtudiant);    
    }
    // Dans EtudiantRepository
    // Dans EtudiantRepository
    public async Task<List<Etudiant>> GetByUEAsync(long ueId)
    {
        return await context.Etudiants
            .Include(e => e.NotesObtenues) // Charge les notes
            .Include(e => e.ParcoursSuivi)
            .Where(e => e.ParcoursSuivi.UesEnseignees.Any(u => u.Id == ueId))
            .ToListAsync();
    }
    // Dans EtudiantRepository
    public async Task<Etudiant?> FindAsync(string numEtud)
    {
        return await context.Etudiants
            .Include(e => e.ParcoursSuivi)          // Charge le parcours
            .ThenInclude(p => p.UesEnseignees)      // Charge les UEs du parcours
            .FirstOrDefaultAsync(e => e.NumEtud == numEtud);
    }
}