using HackathonUsers.Domain.Dto;
using MediatR;
using OperationResult;

namespace HackathonUsers.Application.Auth.Commands;

public sealed class LoginServiceRequest : IRequest<Result<LoginDto>>
{
    public Guid ClientId { get; set; } = Guid.Empty;
    public string ClientSecret { get; set; } = null!;
}