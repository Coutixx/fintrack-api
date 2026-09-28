using System.IdentityModel.Tokens.Jwt;
using DotNetEnv;
using FinTrack.Api.Exceptions;
using FinTrack.Application;
using FinTrack.Infrastructure;
using Scalar.AspNetCore;

Env.TraversePath().Load();
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapOpenApi();
app.MapScalarApiReference();

app.Run();