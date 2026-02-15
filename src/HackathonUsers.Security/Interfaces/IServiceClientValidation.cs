namespace HackathonUsers.Security.Interfaces;

public interface IServiceClientValidation
{
    bool Validate(Guid clientId, string clientSecret);
}