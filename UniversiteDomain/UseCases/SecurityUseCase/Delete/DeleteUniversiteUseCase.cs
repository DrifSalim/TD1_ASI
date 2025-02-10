using UniversiteDomain.UseCases.EtudiantUseCases.Get;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.SecurityUseCase.Delete;

public class DeleteUniversiteUserUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<bool> ExecuteAsync(long etuiantId)
    {
        // Ensure the input is a valid student number (non-zero)
        if (etuiantId== 0)
        {
            throw new ArgumentException("Numéro d'étudiant is required", nameof(etuiantId));
        }

        // Search for the Etudiant by their NumEtud
        var user = await repositoryFactory.UniversiteUserRepository().FindByConditionAsync(e => e.EtudiantId.Equals(etuiantId));
        Console.WriteLine($"Deleting student with NumEtud {etuiantId} &&");

        // If no Etudiant is found, throw an exception
        /*if (!etudiant.Any())
        {
            Console.WriteLine($"Deleting student with NumEtud {numEtud}aaa");

            throw new EtudiantNotFoundException($"A student with NumEtud {numEtud} does not exist.");
        }*/
        Console.WriteLine($"Deleting student with NumEtud {etuiantId}");
        // If the Etudiant exists, delete them
        //var etudiantToDelete = etudiant.First(); // We expect only one result, as NumEtud should be unique
        await repositoryFactory.EtudiantRepository().DeleteAsync(etuiantId);

        // Commit the transaction (save changes to the database)
        await repositoryFactory.SaveChangesAsync();
        
        Console.WriteLine($"Deleting student with NumEtud ana hna f deleteetudusecase sff rah daz");


        return true; // Return true to indicate successful deletion
    }


    // Authorization check to verify if the user has the right role to perform the delete operation
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}