using HackathonUsers.Application.Auth.Commands;
using HackathonUsers.Domain.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HackathonUsers.Api.Controllers;

[Route("api/[controller]/[action]")]
public class AuthController(ILogger<AuthController> logger, IMediator mediator) : BaseController(logger)
{
    [HttpPost]
    [ProducesResponseType(typeof(LoginDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SignIn([FromBody] LoginUserRequest request)
        => await Send(mediator.Send(request));
    
    [HttpPost]
    [ProducesResponseType(typeof(LoginDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Token([FromBody] LoginServiceRequest request)
        => await Send(mediator.Send(request));
}