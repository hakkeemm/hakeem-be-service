using Hakeem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hakeem.Infrastructure.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasOne(r => r.Patient)
            .WithMany(p => p.WrittenReviews)
            .HasForeignKey(r => r.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Doctor)
            .WithMany(d => d.ReceivedReviews)
            .HasForeignKey(r => r.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Appointment)
            .WithOne(a => a.Review)
            .HasForeignKey<Review>(r => r.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
