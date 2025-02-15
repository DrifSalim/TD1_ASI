using UniversiteDomain.Util;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions;
using UniversiteDomain.Exceptions.EtudiantExceptions;

namespace UniversiteDomain.UseCases.SecurityUseCases.Update;

public class UpdateUniversiteUserUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<IUniversiteUser> ExecuteAsync(Etudiant etudiant)
    {
        
        // Ensure the input is not null
        if (etudiant == null)
        {
            throw new ArgumentNullException(nameof(etudiant), "Etudiant cannot be null");
        }

        await CheckBusinessRules(etudiant);

        // Recherche de l'utilisateur existant
        var existingUser = await repositoryFactory.UniversiteUserRepository().FindUserByIdAsync(etudiant.Id);
        if (existingUser == null)
        {
            throw new UserNotFoundException($"Aucun utilisateur trouvé : {etudiant.Id}");
        }

        // Mise à jour uniquement des propriétés nécessaires de l'utilisateur
        
        await repositoryFactory.UniversiteUserRepository().UpdateAsync(
            existingUser,
            etudiant.Nom,
            etudiant.Email
        );

        return existingUser;
    }

    private async Task CheckBusinessRules(Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(etudiant);
        ArgumentNullException.ThrowIfNull(etudiant.Email);
        ArgumentNullException.ThrowIfNull(repositoryFactory);

        // Vérification du format du mail
        if (!CheckEmail.IsValidEmail(etudiant.Email))
        {
            throw new InvalidEmailException(etudiant.Email + " - Email mal formé");
        }

        // Vérification si l'email est déjà utilisé par un autre utilisateur
        var existingUser = await repositoryFactory.UniversiteUserRepository().FindByEmailAsync(etudiant.Email);
        if (existingUser != null && existingUser.EtudiantId != etudiant.Id)
        {
            throw new InvalidEmailException(etudiant.Email + " est déjà utilisé par un autre utilisateur");
        }
    }

    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}