using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace HackathonUsers.Api.Middlewares;

public class GlobalExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ILogger<GlobalExceptionMiddleware> logger)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var correlationId = context.Items["CorrelationId"]?.ToString() ?? string.Empty;
            const string logTemplate = "An unexpected fault happened, please contact your Administrator with the error id: {CorrelationId}.";

            logger.LogError(ex, logTemplate, correlationId);
            
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            
            var problem = new ProblemDetails
            {
                Title = "An unexpected fault happened",
                Status = StatusCodes.Status500InternalServerError,
                Detail = $"An unexpected fault happened, please contact your Administrator with the error id: {correlationId}.",
                Instance = context.Request.Path,
            };
            
            var result = JsonSerializer.Serialize(problem);
            await context.Response.WriteAsync(result);
        }
    }
}