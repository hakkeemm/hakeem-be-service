using Hakeem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hakeem.Infrastructure.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasOne(u => u.Specialty)
            .WithMany(s => s.Doctors)
            .HasForeignKey(u => u.SpecialtyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(u => u.AssignedDoctor)
            .WithMany(d => d.Assistants)
            .HasForeignKey(u => u.AssignedDoctorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
