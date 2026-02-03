using Serilog.Context;

namespace HackathonUsers.Api.Middlewares;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.Headers.TryGetValue("X-Correlation-Id", out var correlationIds);
        var correlationId = correlationIds.FirstOrDefault() ?? Guid.NewGuid().ToString();
        
        context.Items["correlationId"] = correlationId;
        
        NewRelic.Api.Agent.NewRelic
            .GetAgent()
            .CurrentTransaction
            .AddCustomAttribute("correlationId", correlationId);

        using (LogContext.PushProperty("correlationId", correlationId))
        {
            await next(context);
        }
    }
}