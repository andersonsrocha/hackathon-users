using HackathonUsers.Application.Auth.Commands;
using HackathonUsers.Domain.Dto;
using HackathonUsers.Security.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace HackathonUsers.Application.Auth.Handlers;

public class LoginServiceHandler(IJwtService jwtService, IServiceClientValidation clientValidator, ILogger<LoginServiceHandler> logger) : IRequestHandler<LoginServiceRequest, Result<LoginDto>>
{
    public Task<Result<LoginDto>> Handle(LoginServiceRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to authenticate service client {ClientId}", request.ClientId);
        if (request.ClientId == Guid.Empty)
            return Task.FromResult(Result.Error<LoginDto>(new Exception("ClientId is required")));
        
        if (string.IsNullOrEmpty(request.ClientSecret))
            return Task.FromResult(Result.Error<LoginDto>(new Exception("ClientSecret is required")));
        
        logger.LogInformation("Verifying client credentials");
        if (!clientValidator.Validate(request.ClientId, request.ClientSecret))
        {
            logger.LogWarning("Invalid service credentials for {ClientId}", request.ClientId);
            return Task.FromResult(Result.Error<LoginDto>(new Exception("Invalid client credentials")));
        }
        
        logger.LogInformation("Service client {ClientId} authenticated successfully, generate JWT token.", request.ClientId);
        var token = jwtService.Generate(request.ClientId);
        return Task.FromResult(Result.Success(new LoginDto(token, DateTime.UtcNow.AddHours(10).Minute)));
    }
}