using System.Net;
using Microsoft.AspNetCore.Mvc;
using OperationResult;

namespace HackathonUsers.Api.Controllers;

[ApiController]
public class BaseController(ILogger<BaseController> logger) : ControllerBase
{
    protected IActionResult Send(object? value)
        => value switch
        {
            null => Problem(statusCode: (int)HttpStatusCode.NotFound),
            _ => Ok(value)
        };

    protected async Task<IActionResult> Send(Task<Result<Guid>> task)
        => await task switch
        {
            (true, var result) => Created(string.Empty, result),
            (false, _, var exception) => TreatError(exception),
            _ => Problem(statusCode: (int)HttpStatusCode.InternalServerError)
        };
    
    protected async Task<IActionResult> Send<TResponse>(Task<Result<TResponse>> task)
        => await task switch
        {
            (true, var result) => Ok(result),
            (false, _, var exception) => TreatError(exception),
            _ => Problem(statusCode: (int)HttpStatusCode.InternalServerError)
        };
    
    protected async Task<IActionResult> Send(Task<Result> task)
        => await task switch
        {
            (true, _) => Ok(),
            (false, var exception) => TreatError(exception)
        };

    [NonAction]
    private ObjectResult TreatError(Exception? error)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
        switch (error)
        {
            case not null:
                logger.LogError("{ErrorMessage}. Please contact your Administrator with the error id: {CorrelationId}", error.Message, correlationId);
                return Problem(detail: $"{error.Message}. Please contact your Administrator with the error id: {correlationId}", statusCode: StatusCodes.Status400BadRequest);
            default:
                logger.LogError("An unexpected fault happened, please contact your Administrator with the error id: {CorrelationId}", correlationId);
                return Problem(statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}