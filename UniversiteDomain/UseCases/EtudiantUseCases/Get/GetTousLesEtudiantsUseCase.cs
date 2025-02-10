using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Get;

public class GetTousLesEtudiantsUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<List<Etudiant?>> ExecuteAsync()
    {
        await CheckBusinessRules();
        List<Etudiant?> etudiants = await repositoryFactory.EtudiantRepository().FindAllAsync();
        if (etudiants.Count == 0)
        { 
            throw new InvalidOperationException("Failed to retrieve students; result is null.");
            
        }
        return etudiants;
    }
    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        IEtudiantRepository etudiantRepository=repositoryFactory.EtudiantRepository();
        ArgumentNullException.ThrowIfNull(etudiantRepository);
    }
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }

    
}