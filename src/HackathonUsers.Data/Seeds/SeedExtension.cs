using Microsoft.Extensions.DependencyInjection;

namespace HackathonUsers.Data.Seeds;

public static class SeedExtension
{
    public static void AddSeeds(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;
        
        provider.AddRoleSeed();
        provider.AddAdminSeed();
    }
}