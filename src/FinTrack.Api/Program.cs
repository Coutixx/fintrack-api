using DotNetEnv;

using FinTrack.Infrastructure;
using FinTrack.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();


app.Run();
