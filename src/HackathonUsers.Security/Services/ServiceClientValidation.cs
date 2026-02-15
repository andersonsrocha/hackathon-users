using HackathonUsers.Security.Interfaces;
using HackathonUsers.Security.Models;
using Microsoft.Extensions.Options;

namespace HackathonUsers.Security.Services;

public class ServiceClientValidation(IOptions<ServiceClientOptions> options) : IServiceClientValidation
{
    public bool Validate(Guid clientId, string clientSecret)
    {
        if (!options.Value.Clients.TryGetValue(clientId.ToString(), out var client))
            return false;

        return clientSecret == client.Secret;
    }
}