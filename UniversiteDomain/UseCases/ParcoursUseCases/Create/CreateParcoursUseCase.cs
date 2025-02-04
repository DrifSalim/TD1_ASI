using System.IO.Pipes;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursException;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Create;

public class CreateParcoursUseCase (IRepositoryFactory repositoryFactory)
{
    public async Task<Parcours> ExecuteAsync(string parcoursName, int year)
    {
        var parcours = new Parcours{NomParcours = parcoursName, AnneeFormation = year};
        return await ExecuteAsync(parcours);
    }

    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        Parcours p = await repositoryFactory.ParcoursRepository().CreateAsync(parcours);
        return p;
    }

    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        ArgumentNullException.ThrowIfNull(parcours.AnneeFormation);
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentNullException.ThrowIfNull(repositoryFactory.ParcoursRepository());
        // On recherche un parcours avec le même id
        List<Parcours> existe = await repositoryFactory.ParcoursRepository().FindByConditionAsync(p=>p.NomParcours.Equals(parcours.NomParcours));
        // Si un parcour existe déjà, on lève une exception personnalisée
        if (existe is {Count:>0}) throw new DuplicateNomParcoursException(parcours.NomParcours+ " - ce parcour existe déja !");
    }
}