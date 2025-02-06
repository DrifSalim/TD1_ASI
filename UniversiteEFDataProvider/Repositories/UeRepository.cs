using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteEFDataProvider.Data;
using UniversiteDomain.Entities;

namespace UniversiteEFDataProvider.Repositories;

public class UeRepository(UniversiteDbContext context) : Repository<Ue>(context), IUeRepository
{
    
}