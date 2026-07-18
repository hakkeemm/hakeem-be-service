using Hakeem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hakeem.Infrastructure.Data.Configurations;

public class DoctorAttachmentConfiguration : IEntityTypeConfiguration<DoctorAttachment>
{
    public void Configure(EntityTypeBuilder<DoctorAttachment> builder)
    {
        builder.HasOne(a => a.Doctor)
            .WithMany(d => d.Attachments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
