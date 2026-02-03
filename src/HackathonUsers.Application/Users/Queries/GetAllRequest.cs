using HackathonUsers.Domain.Dto;
using MediatR;
using OperationResult;

namespace HackathonUsers.Application.Users.Queries;

public sealed class GetAllRequest : IRequest<Result<IEnumerable<UserDto>>>;