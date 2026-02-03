using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using HackathonUsers.Domain.Models;

namespace HackathonUsers.Data.Seeds;

public static class AdminSeed
{
    private const string AdminEmail = "admin@fiap.com.br";

    public static void AddAdminSeed(this IServiceProvider provider)
    {
        var userManager = provider.GetRequiredService<UserManager<User>>();
        
        var admin = userManager.FindByEmailAsync(AdminEmail).Result;
        if (admin is null)
        {
            admin = new User("Admin", AdminEmail);
            var result = userManager.CreateAsync(admin, "*_7hg613").Result;
            if (result.Succeeded)
                userManager.AddToRoleAsync(admin, "Admin").Wait();
        }
    }
}