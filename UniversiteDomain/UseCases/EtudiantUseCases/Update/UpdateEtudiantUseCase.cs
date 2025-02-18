using UniversiteDomain.Util;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Update;

public class UpdateEtudiantUseCase(IRepositoryFactory etudiantRepository)
{
    public async Task<Etudiant> ExecuteAsync(Etudiant etudiant)
    {
        if (etudiant == null)
        {
            throw new ArgumentNullException(nameof(etudiant), "Etudiant cannot be null");
        }
        List<Etudiant> etudiantToUpdate = await etudiantRepository.EtudiantRepository().FindByConditionAsync(e => e.Id == etudiant.Id);
        if (etudiantToUpdate is { Count: 0 } || etudiant is null) throw new EtudiantNotFoundException(etudiant.Id.ToString());

        var existingStudent = etudiantToUpdate[0];
        
        // Apply changes from the input to the existing entity
        existingStudent.NumEtud = etudiant.NumEtud;
        existingStudent.Nom = etudiant.Nom;
        existingStudent.Prenom = etudiant.Prenom;
        existingStudent.Email = etudiant.Email;

        // Validate the updated entity
        await CheckBusinessRules(existingStudent);

        await etudiantRepository.EtudiantRepository().UpdateAsync(existingStudent);
        etudiantRepository.SaveChangesAsync().Wait();
        return existingStudent;
    }

    private async Task CheckBusinessRules(Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(etudiant);
        ArgumentNullException.ThrowIfNull(etudiant.NumEtud);
        ArgumentNullException.ThrowIfNull(etudiant.Email);
        ArgumentNullException.ThrowIfNull(etudiantRepository);
        
        // Check for duplicate NumEtud excluding the current student
        var existingWithNumEtud = await etudiantRepository.EtudiantRepository()
            .FindByConditionAsync(e => e.NumEtud == etudiant.NumEtud && e.Id != etudiant.Id);
        if (existingWithNumEtud.Any())
            throw new DuplicateNumEtudException($"{etudiant.NumEtud} - Ce numéro étudiant est déjà utilisé par un autre étudiant.");

        // Validate email format
        if (!CheckEmail.IsValidEmail(etudiant.Email))
            throw new InvalidEmailException($"{etudiant.Email} - Format d'email invalide.");

        // Check for duplicate Email excluding the current student
        var existingWithEmail = await etudiantRepository.EtudiantRepository()
            .FindByConditionAsync(e => e.Email == etudiant.Email && e.Id != etudiant.Id);
        if (existingWithEmail.Any())
            throw new InvalidEmailException($"{etudiant.Email} - Cet email est déjà utilisé par un autre étudiant.");

        // Validate name length
        if (etudiant.Nom.Length < 3)
            throw new InvalidNomEtudiantException($"{etudiant.Nom} - Le nom doit contenir au moins 3 caractères.");
    }

    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}