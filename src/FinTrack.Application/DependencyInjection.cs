using FinTrack.Application.Common.Behaviors;
using FinTrack.Application.Features.Auth;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FinTrack.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(LoginCommand).Assembly);
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly);
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        return services;
    }
}
