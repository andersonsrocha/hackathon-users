using HackathonUsers.Api.Configurations;
using HackathonUsers.Api.Middlewares;
using HackathonUsers.Application;
using HackathonUsers.Data;
using HackathonUsers.Data.Seeds;
using HackathonUsers.Security;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

#region [Database]
builder.Services.AddSqlContext(builder.Configuration);
builder.Services.AddRepositories();
#endregion

#region [JWT]
builder.Services.AddSecurity(builder.Configuration);
#endregion

#region [AutoMapper]
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
#endregion

#region [Mediator]
builder.Services.AddMediator();
#endregion

#region [Serilog]
builder.AddSerilog();
#endregion

var app = builder.Build();

app.MapSwagger("/openapi/{documentName}.json");
app.MapScalarApiReference();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<RequestMiddleware>();

#region [Seed]
app.Services.AddSeeds();
#endregion

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();