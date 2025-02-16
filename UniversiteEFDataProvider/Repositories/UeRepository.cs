using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteEFDataProvider.Data;
using UniversiteDomain.Entities;

namespace UniversiteEFDataProvider.Repositories;

public class UeRepository(UniversiteDbContext context) : Repository<Ue>(context), IUeRepository
{
    public async Task<Ue?> GetByNumeroAsync(string numeroUe)
    {
        return await context.Ues
            .FirstOrDefaultAsync(u => u.NumeroUe == numeroUe); // <-- Requête par NumeroUe (string)
    }
}