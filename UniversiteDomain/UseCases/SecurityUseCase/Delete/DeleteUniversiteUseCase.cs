using UniversiteDomain.UseCases.EtudiantUseCases.Get;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions;
using UniversiteDomain.Exceptions.EtudiantExceptions;

namespace UniversiteDomain.UseCases.SecurityUseCase.Delete;

public class DeleteUniversiteUserUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<bool> ExecuteAsync(long etudiantId)
    {
        if (etudiantId <= 0)
        {
            throw new ArgumentException("Numéro d'étudiant positif", nameof(etudiantId));
        }

        List<Etudiant> etud = await repositoryFactory.EtudiantRepository()
            .FindByConditionAsync(e => e.Id == etudiantId);
        if (etud.Count == 0)
        {
            throw new EtudiantNotFoundException("User not found");
        }
        var user= await repositoryFactory.UniversiteUserRepository().FindByEmailAsync(etud[0].Email);
        if (user == null)
        {
            throw new UserNotFoundException($"Le user: {etudiantId} n'existe pas.");
        }

        await repositoryFactory.UniversiteUserRepository().DeleteAsync(user);
        await repositoryFactory.SaveChangesAsync();

        return true;
    }


    // Authorization check to verify if the user has the right role to perform the delete operation
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}

