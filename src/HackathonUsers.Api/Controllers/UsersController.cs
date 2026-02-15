using HackathonUsers.Application.Users.Commands;
using HackathonUsers.Application.Users.Queries;
using HackathonUsers.Domain.Dto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HackathonUsers.Api.Controllers;

[Route("api/[controller]")]
public class UsersController(ILogger<UsersController> logger, IMediator mediator) : BaseController(logger)
{
    [HttpGet]
    [Authorize("Admin")]
    [Route("{id:Guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
        => await Send(mediator.Send(new GetByIdRequest(id)));
    
    [HttpGet]
    [Authorize("Admin")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get()
        => await Send(mediator.Send(new GetAllRequest()));
    
    [HttpPost]
    [Authorize("Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateUserRequest request)
        => await Send(mediator.Send(request));
}