using UniversiteDomain.Entities;
namespace UniversiteDomain.DataAdapters;

public interface IUniversiteUserRepository : IRepository<IUniversiteUser>
{
    Task<IUniversiteUser?> AddUserAsync(string login, string email, string password, string role, Etudiant? etudiant);
    Task<IUniversiteUser> FindByEmailAsync(string email);
    Task<bool> IsInRoleAsync(string email, string role);
    Task UpdateAsync(IUniversiteUser entity, string userName, string email);
    Task<IUniversiteUser?> FindUserByIdAsync(long id);
}