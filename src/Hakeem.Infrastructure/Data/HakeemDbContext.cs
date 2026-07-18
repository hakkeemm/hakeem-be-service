using Hakeem.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hakeem.Infrastructure.Data;

public class HakeemDbContext : IdentityDbContext<ApplicationUser>
{
    public HakeemDbContext(DbContextOptions<HakeemDbContext> options) : base(options)
    {
    }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Category> Specialties { get; set; }
    public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
    public DbSet<DoctorScheduleException> DoctorScheduleExceptions { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<OnlineMeeting> OnlineMeetings { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<DoctorAttachment> DoctorAttachments { get; set; }
    public DbSet<Favorite> Favorites { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // This will automatically find and apply all classes that implement IEntityTypeConfiguration<T>
        builder.ApplyConfigurationsFromAssembly(typeof(HakeemDbContext).Assembly);
    }
}
