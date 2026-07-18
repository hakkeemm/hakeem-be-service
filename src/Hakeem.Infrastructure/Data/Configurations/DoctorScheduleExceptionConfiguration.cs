using Hakeem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hakeem.Infrastructure.Data.Configurations;

public class DoctorScheduleExceptionConfiguration : IEntityTypeConfiguration<DoctorScheduleException>
{
    public void Configure(EntityTypeBuilder<DoctorScheduleException> builder)
    {
        builder.HasOne(e => e.Doctor)
            .WithMany(d => d.ScheduleExceptions)
            .HasForeignKey(e => e.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
