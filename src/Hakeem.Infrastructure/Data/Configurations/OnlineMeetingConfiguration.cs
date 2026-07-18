using Hakeem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hakeem.Infrastructure.Data.Configurations;

public class OnlineMeetingConfiguration : IEntityTypeConfiguration<OnlineMeeting>
{
    public void Configure(EntityTypeBuilder<OnlineMeeting> builder)
    {
        builder.HasOne(m => m.Appointment)
            .WithOne(a => a.OnlineMeeting)
            .HasForeignKey<OnlineMeeting>(m => m.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
