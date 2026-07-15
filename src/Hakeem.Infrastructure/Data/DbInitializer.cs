using Hakeem.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hakeem.Infrastructure.Data;

public interface IDbInitializer
{
    Task InitializeAsync();
}

public class DbInitializer : IDbInitializer
{
    private readonly HakeemDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(HakeemDbContext context, RoleManager<IdentityRole> roleManager, ILogger<DbInitializer> logger)
    {
        _context = context;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            // Apply pending migrations
            if (_context.Database.GetPendingMigrations().Any())
            {
                await _context.Database.MigrateAsync();
            }

            // Seed roles if the roles table is empty
            if (!await _roleManager.Roles.AnyAsync())
            {
                var roles = Enum.GetNames(typeof(UserRole));
                foreach (var role in roles)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }
}
