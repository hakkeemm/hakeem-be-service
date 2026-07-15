using System.Text;
using Hakeem.Domain.Entities;
using Hakeem.Domain.Interfaces;
using Hakeem.Infrastructure.Data;
using Hakeem.Infrastructure.Email;
using Hakeem.Infrastructure.Identity;
using Hakeem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Hakeem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<HakeemDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
        })
        .AddEntityFrameworkStores<HakeemDbContext>()
        .AddDefaultTokenProviders();

        var jwtSettings = configuration.GetSection("JwtSettings");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured"));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IDbInitializer, DbInitializer>();

        return services;
    }
}
