using HackathonUsers.Domain.Dto;
using MediatR;
using OperationResult;

namespace HackathonUsers.Application.Users.Commands;

public sealed class LoginUserRequest : IRequest<Result<LoginDto>>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}