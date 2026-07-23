using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace FinTrack.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetCallingAssembly()));

        return services;
    }
}
