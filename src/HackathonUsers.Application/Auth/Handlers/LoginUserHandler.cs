using HackathonUsers.Application.Auth.Commands;
using HackathonUsers.Domain.Dto;
using HackathonUsers.Domain.Models;
using HackathonUsers.Security.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace HackathonUsers.Application.Auth.Handlers;

public class LoginUserHandler(SignInManager<User> signInManager, IJwtService jwtService, ILogger<LoginUserHandler> logger) : IRequestHandler<LoginUserRequest, Result<LoginDto>>
{
    public async Task<Result<LoginDto>> Handle(LoginUserRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to sign in user with email: {Email}", request.Email);
        var user = await signInManager.UserManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Error<LoginDto>(new Exception("User not found"));
        
        logger.LogInformation("User found. Verifying password.");
        var result = await signInManager.PasswordSignInAsync(user, request.Password, false, true);
        if (result.IsNotAllowed)
            return Result.Error<LoginDto>(new Exception("User not allowed to login"));
        
        logger.LogInformation("User is allowed.");
        if (result.IsLockedOut)
            return Result.Error<LoginDto>(new Exception("User is locked out"));
        
        logger.LogInformation("User is not locked out.");
        if (!result.Succeeded)
            return Result.Error<LoginDto>(new Exception("User or password is incorrect"));
        
        logger.LogInformation("Password verified. Retrieving user roles.");
        var roles = await signInManager.UserManager.GetRolesAsync(user);
        
        logger.LogInformation("Roles retrieved. Generate JWT token.");
        var token = jwtService.Generate(user, roles);
        return Result.Success(new LoginDto(token, DateTime.UtcNow.AddHours(5).Minute));
    }
}