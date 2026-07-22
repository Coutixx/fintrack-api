using DotNetEnv;

using FinTrack.Infrastructure;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.Run();
