using System.ComponentModel.DataAnnotations;
using AutoMapper;
using HackathonUsers.Application.Users.Commands;
using HackathonUsers.Domain.Dto;
using HackathonUsers.Domain.Interfaces;
using HackathonUsers.Domain.Models;
using HackathonUsers.Security;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace HackathonUsers.Application.Users.Handlers;

public class CreateUserHandler(UserManager<User> userManager, IUserStore<User> userStore, IMapper mapper, ILogger<CreateUserHandler> logger) : IRequestHandler<CreateUserRequest, Result<UserDto>>
{
    private static readonly EmailAddressAttribute EmailAddressAttribute = new();
    
    public async Task<Result<UserDto>> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating a new user with email: {Email}", request.Email);
        if (!userManager.SupportsUserEmail)
            throw new NotSupportedException($"{nameof(CreateUserHandler)} requires a user store with email support.");

        var user = (User)request;
        var emailStore = (IUserEmailStore<User>)userStore;
        var email = request.Email;

        logger.LogInformation("Validating email format.");
        if (string.IsNullOrEmpty(email) || !EmailAddressAttribute.IsValid(email))
            return Result.Error<UserDto>(new Exception(userManager.ErrorDescriber.InvalidEmail(email).Description));

        logger.LogInformation("Checking if password is valid.");
        var passwordIsValid =
            await userManager.PasswordValidators[0].ValidateAsync(userManager, user, request.Password);
        if (!passwordIsValid.Succeeded)
            return Result.Error<UserDto>(new Exception(passwordIsValid.Errors.First().Description));

        await userStore.SetUserNameAsync(user, request.Email, CancellationToken.None);
        await emailStore.SetEmailAsync(user, request.Email, CancellationToken.None);
        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return Result.Error<UserDto>(new Exception(result.Errors.First().Description));

        logger.LogInformation("User created successfully. Assigning 'User' role.");
        await userManager.AddToRoleAsync(user, "User");

        return Result.Success(mapper.Map<UserDto>(user));
    }
}