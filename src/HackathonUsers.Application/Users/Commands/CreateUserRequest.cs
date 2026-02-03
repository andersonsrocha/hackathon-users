using HackathonUsers.Domain.Dto;
using HackathonUsers.Domain.Models;
using MediatR;
using OperationResult;

namespace HackathonUsers.Application.Users.Commands;

public sealed class CreateUserRequest : IRequest<Result<UserDto>>
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;

    public static implicit operator User(CreateUserRequest request)
        => new User(request.Name, request.Email);
}