using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.SecurityUseCase.Get;
public class FindUniversiteUserByIdUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<IUniversiteUser?> ExecuteAsync(long id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid user ID", nameof(id));
        }

        var user = await repositoryFactory.UniversiteUserRepository()
            .FindUserByIdAsync(id);

        return user;
    }
}