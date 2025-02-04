using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.UeUseCases.Create;

public class CreateUeUseCase (IRepositoryFactory factory)
{
    
    public async Task<Ue> ExecuteAsync(string numUe, string libelle)
    {
        Ue ue = new Ue{NumeroUe = numUe, Intitule = libelle};
        return await ExecuteAsync(ue);
    }
    
    public async Task<Ue> ExecuteAsync(Ue ue)
    {
        await CheckBusinessRules(ue);
        Ue u = await factory.UeRepository().CreateAsync(ue);
        return u;
    }

    private async Task CheckBusinessRules(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(ue.Intitule);
        ArgumentNullException.ThrowIfNull(ue.NumeroUe);
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(factory.UeRepository());
        if (ue.Intitule.Length <= 3) throw new IntituleException("L'intitulé doit avoir au moins 4 caractères");
        //Test si deux ues ne portent pas le même numero
        List<Ue> existe = await factory.UeRepository().FindByConditionAsync(u=>u.NumeroUe.Equals(ue.NumeroUe));
        if (existe is { Count: > 0 }) throw new DuplicateNumUeException("Numero Ue Existe Déja");
        
    }
    
}