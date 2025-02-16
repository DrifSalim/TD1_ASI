using System.Linq.Expressions;
using UniversiteDomain.Entities;
 
namespace UniversiteDomain.DataAdapters;
 
public interface IEtudiantRepository : IRepository<Etudiant>
{
    public Task<Etudiant?> FindEtudiantCompletAsync(long idEtudiant);
    public Task<List<Etudiant>> GetByUEAsync(long ueId);
    Task<Etudiant?> FindAsync(string numEtud);

}