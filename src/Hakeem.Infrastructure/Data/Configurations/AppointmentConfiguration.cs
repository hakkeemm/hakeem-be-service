using Hakeem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hakeem.Infrastructure.Data.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasOne(a => a.Patient)
            .WithMany(p => p.BookedAppointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Doctor)
            .WithMany(d => d.ReceivedAppointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Decimal precision for fees
        builder.Property(a => a.Fee)
            .HasColumnType("decimal(18,2)");

        // Explicitly enforce that Enums are stored as Integers (Numbers)
        builder.Property(a => a.VisitType)
            .HasConversion<int>();

        builder.Property(a => a.VisitPurpose)
            .HasConversion<int>();

        builder.Property(a => a.Status)
            .HasConversion<int>();

    }
}
