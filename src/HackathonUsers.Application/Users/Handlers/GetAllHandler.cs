using AutoMapper;
using HackathonUsers.Application.Users.Queries;
using HackathonUsers.Domain.Dto;
using HackathonUsers.Domain.Interfaces;
using MediatR;
using OperationResult;

namespace HackathonUsers.Application.Users.Handlers;

public class GetAllHandler(IUserRepository repository, IMapper mapper) : IRequestHandler<GetAllRequest, Result<IEnumerable<UserDto>>>
{
    public async Task<Result<IEnumerable<UserDto>>> Handle(GetAllRequest request, CancellationToken cancellationToken)
        => Result.Success(mapper.Map<IEnumerable<UserDto>>(await repository.Find(cancellationToken)));
}