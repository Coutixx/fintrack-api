using FinTrack.Application.Common.Interfaces;
using FinTrack.Infrastructure.Data;
using FinTrack.Infrastructure.Security;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinTrack.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(
    configuration.GetConnectionString("PostgresConnection")));

        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
