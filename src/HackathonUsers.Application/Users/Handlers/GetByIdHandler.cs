using AutoMapper;
using HackathonUsers.Application.Users.Queries;
using HackathonUsers.Domain.Dto;
using HackathonUsers.Domain.Interfaces;
using MediatR;
using OperationResult;

namespace HackathonUsers.Application.Users.Handlers;

public class GetByIdHandler(IUserRepository repository, IMapper mapper) : IRequestHandler<GetByIdRequest, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetByIdRequest request, CancellationToken cancellationToken)
        => Result.Success(mapper.Map<UserDto>(await repository.Find(request.Id, cancellationToken)));
}