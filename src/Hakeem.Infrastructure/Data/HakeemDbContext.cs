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
        
        builder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .IsRequired();
            
        builder.Entity<RefreshToken>()
            .HasIndex(rt => rt.Token)
            .IsUnique();

        // ApplicationUser configurations
        builder.Entity<ApplicationUser>()
            .HasOne(u => u.Specialty)
            .WithMany(s => s.Doctors)
            .HasForeignKey(u => u.SpecialtyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<ApplicationUser>()
            .HasOne(u => u.AssignedDoctor)
            .WithMany(d => d.Assistants)
            .HasForeignKey(u => u.AssignedDoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        // DoctorSchedule
        builder.Entity<DoctorSchedule>()
            .HasOne(s => s.Doctor)
            .WithMany(d => d.Schedules)
            .HasForeignKey(s => s.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);

        // DoctorScheduleException
        builder.Entity<DoctorScheduleException>()
            .HasOne(e => e.Doctor)
            .WithMany(d => d.ScheduleExceptions)
            .HasForeignKey(e => e.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);


        // Appointment
        builder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany(p => p.BookedAppointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.ReceivedAppointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);


        // OnlineMeeting
        builder.Entity<OnlineMeeting>()
            .HasOne(m => m.Appointment)
            .WithOne(a => a.OnlineMeeting)
            .HasForeignKey<OnlineMeeting>(m => m.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Review
        builder.Entity<Review>()
            .HasOne(r => r.Patient)
            .WithMany(p => p.WrittenReviews)
            .HasForeignKey(r => r.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Review>()
            .HasOne(r => r.Doctor)
            .WithMany(d => d.ReceivedReviews)
            .HasForeignKey(r => r.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Review>()
            .HasOne(r => r.Appointment)
            .WithOne(a => a.Review)
            .HasForeignKey<Review>(r => r.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // DoctorAttachment
        builder.Entity<DoctorAttachment>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.Attachments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Favorite
        builder.Entity<Favorite>()
            .HasOne(f => f.Patient)
            .WithMany(p => p.SavedFavorites)
            .HasForeignKey(f => f.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Favorite>()
            .HasOne(f => f.Doctor)
            .WithMany(d => d.FavoritedBy)
            .HasForeignKey(f => f.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
