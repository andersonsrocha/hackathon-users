using Serilog.Context;

namespace HackathonUsers.Api.Middlewares;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.Headers.TryGetValue("X-Correlation-Id", out var correlationIds);
        var correlationId = correlationIds.FirstOrDefault() ?? Guid.NewGuid().ToString();
        
        context.Items["CorrelationId"] = correlationId;
        
        NewRelic.Api.Agent.NewRelic
            .GetAgent()
            .CurrentTransaction
            .AddCustomAttribute("CorrelationId", correlationId);

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }
}