using Hakeem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hakeem.Infrastructure.Data.Configurations;

public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.HasOne(f => f.Patient)
            .WithMany(p => p.SavedFavorites)
            .HasForeignKey(f => f.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Doctor)
            .WithMany(d => d.FavoritedBy)
            .HasForeignKey(f => f.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
