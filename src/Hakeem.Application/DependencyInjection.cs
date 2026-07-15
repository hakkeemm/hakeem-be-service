using FluentValidation;
using Hakeem.Application.Interfaces;
using Hakeem.Application.Services;
using Hakeem.Application.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Hakeem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        
        return services;
    }
}
