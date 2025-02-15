using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.Entities;

namespace UniversiteEFDataProvider.Repositories;

public class UniversiteUserRepository(UniversiteDbContext context, UserManager<UniversiteUser> userManager, RoleManager<UniversiteRole> roleManager) : Repository<IUniversiteUser>(context), IUniversiteUserRepository
{
    public async Task<IUniversiteUser?> AddUserAsync(string login, string email, string password, string role,  Etudiant? etudiant)
    {
        UniversiteUser user = new UniversiteUser { UserName = login, Email = email, Etudiant = etudiant };
        IdentityResult result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, role);
        }
        await context.SaveChangesAsync();
        return result.Succeeded ? user : null;
        return user;
    }

    public async Task<IUniversiteUser> FindByEmailAsync(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }
    
    public async Task UpdateAsync(IUniversiteUser entity, string userName, string email)
    {
        UniversiteUser user = (UniversiteUser)entity;
        user.UserName = userName;
        user.Email = email;
        await userManager.UpdateAsync(user);
        await context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(UniversiteUser user)
    {
        
        if (user!=null)
        {
            await userManager.DeleteAsync(user);
            int res=await  context.SaveChangesAsync();
        }
        else
        {
            throw new UserNotFoundException($"L'utilisateur {user} n'existe pas.");
        }
    }

    public async Task<bool> IsInRoleAsync(string email, string role)
    {
        bool res = false;
        var user =await userManager.FindByEmailAsync(email);
        return await userManager.IsInRoleAsync(user, role);
    }

    public async Task<IUniversiteUser?> FindUserByIdAsync(long etudiantId)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Etudiant.Id== etudiantId);
    }
}