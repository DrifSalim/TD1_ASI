using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.UeUseCases.Create;
namespace UniversiteDomainUnitTests;

public class UeUnitTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateUeUseCase()
    {
        long id = 1;
        string numeroUe = "1234";
        string intituleUe = "1234";
        // On crée l'ue qui doit être ajouté en base
        Ue ueSansId = new Ue{NumeroUe = numeroUe, Intitule = intituleUe};
        //  Créons le mock du repository
        // On initialise une fausse datasource qui va simuler un UeRepository
        var mock = new Mock<IUeRepository>();
        // Il faut ensuite aller dans le use case pour voir quelles fonctions simuler
        // Nous devons simuler FindByCondition et Create
        // Simulation de la fonction FindByCondition
        // On dit à ce mock que l'étudiant n'existe pas déjà
        // La réponse à l'appel FindByCondition est donc une liste vide
        var reponseFindByCondition = new List<Ue>();
        // On crée un bouchon dans le mock pour la fonction FindByCondition
        // Quelque soit le paramètre de la fonction FindByCondition, on renvoie la liste vide
        mock.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(reponseFindByCondition);
        // Simulation de la fonction Create
        // On lui dit que l'ajout d'une ue renvoie une ue avec l'Id 1
        Ue ueCree = new Ue { Id = id, NumeroUe = numeroUe, Intitule = intituleUe };
        mock.Setup(repoUe=>repoUe.CreateAsync(ueSansId)).ReturnsAsync(ueCree);
        
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.UeRepository()).Returns(mock.Object);
        CreateUeUseCase useCase = new CreateUeUseCase(mockFactory.Object);
        var ueTeste=await useCase.ExecuteAsync(ueSansId);
        // Vérification du résultat
        Assert.That(ueTeste.Id, Is.EqualTo(ueCree.Id));
        Assert.That(ueTeste.NumeroUe, Is.EqualTo(ueSansId.NumeroUe));
        Assert.That(ueTeste.Intitule, Is.EqualTo(ueCree.Intitule));

    }
}