using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteEFDataProvider.Data;
using UniversiteDomain.Entities;

namespace UniversiteEFDataProvider.Repositories;

public class ParcoursRepository(UniversiteDbContext context) : Repository<Parcours>(context), IParcoursRepository
{
    public async Task<Parcours> AddEtudiantAsync(long idParcours, long idEtudiant)
    {
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        ArgumentNullException.ThrowIfNull(Context.Etudiants);

        Parcours? parcours = await Context.Parcours.FindAsync(idParcours);
        Etudiant? etudiant = await Context.Etudiants.FindAsync(idEtudiant);

        if (parcours == null || etudiant == null)
            throw new ArgumentException("Parcours ou étudiant introuvable");

        parcours.Inscrits.Add(etudiant);
        await Context.SaveChangesAsync();
        
        return parcours;
    }
    
    public async Task<Parcours> AddEtudiantAsync(Parcours parcours, Etudiant etudiant)
    {
        return await AddEtudiantAsync(parcours.Id, etudiant.Id);
    }
    
    public async Task<Parcours> AddEtudiantAsync(Parcours parcours, List<Etudiant> etudiants)
    {
        foreach (var etudiant in etudiants)
        {
            await AddEtudiantAsync(parcours, etudiant);
        }
        return parcours;
    }
    
    public async Task<Parcours> AddEtudiantAsync(long idParcours, long[] idEtudiants)
    {
        Parcours? parcours = await Context.Parcours.FindAsync(idParcours);
        if (parcours == null)
            throw new ArgumentException("Parcours introuvable");

        foreach (var idEtudiant in idEtudiants)
        {
            Etudiant? etudiant = await Context.Etudiants.FindAsync(idEtudiant);
            if (etudiant != null)
            {
                parcours.Inscrits.Add(etudiant);
            }
        }
        await Context.SaveChangesAsync();
        return parcours;
    }
    
    public async Task<Parcours> AddUeAsync(long idParcours, long idUe)
    {
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        ArgumentNullException.ThrowIfNull(Context.Ues);

        Parcours? parcours = await Context.Parcours.FindAsync(idParcours);
        Ue? ue = await Context.Ues.FindAsync(idUe);

        if (parcours == null || ue == null)
            throw new ArgumentException("Parcours ou UE introuvable");

        parcours.UesEnseignees.Add(ue);
        await Context.SaveChangesAsync();
        
        return parcours;
    }
    
    public async Task<Parcours> AddUeAsync(Parcours parcours, Ue ue)
    {
        return await AddUeAsync(parcours.Id, ue.Id);
    }
    
    public async Task<Parcours> AddUeAsync(Parcours parcours, List<Ue> ues)
    {
        foreach (var ue in ues)
        {
            await AddUeAsync(parcours, ue);
        }
        return parcours;
    }
    
    public async Task<Parcours> AddUeAsync(long idParcours, long[] idUes)
    {
        Parcours? parcours = await Context.Parcours.FindAsync(idParcours);
        if (parcours == null)
            throw new ArgumentException("Parcours introuvable");

        foreach (var idUe in idUes)
        {
            Ue? ue = await Context.Ues.FindAsync(idUe);
            if (ue != null)
            {
                parcours.UesEnseignees.Add(ue);
            }
        }
        await Context.SaveChangesAsync();
        return parcours;
    }
}
