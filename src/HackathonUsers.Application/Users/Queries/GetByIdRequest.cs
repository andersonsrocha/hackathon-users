using HackathonUsers.Domain.Dto;
using MediatR;
using OperationResult;

namespace HackathonUsers.Application.Users.Queries;

public sealed class GetByIdRequest(Guid id) : IRequest<Result<UserDto>>
{
    public Guid Id { get; init; } = id;
}