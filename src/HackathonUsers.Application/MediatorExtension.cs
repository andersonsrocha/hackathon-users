using Microsoft.Extensions.DependencyInjection;

namespace HackathonUsers.Application;

public static class MediatorExtension
{
    public static void AddMediator(this IServiceCollection services) 
        => services.AddMediatR(options => options.RegisterServicesFromAssembly(typeof(MediatorExtension).Assembly));
}