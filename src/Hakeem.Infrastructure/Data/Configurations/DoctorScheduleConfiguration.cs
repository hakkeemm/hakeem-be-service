using Hakeem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hakeem.Infrastructure.Data.Configurations;

public class DoctorScheduleConfiguration : IEntityTypeConfiguration<DoctorSchedule>
{
    public void Configure(EntityTypeBuilder<DoctorSchedule> builder)
    {
        builder.HasOne(s => s.Doctor)
            .WithMany(d => d.Schedules)
            .HasForeignKey(s => s.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
