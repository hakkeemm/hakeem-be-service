using Hakeem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hakeem.Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // Use a fixed date to prevent EF Migrations from generating a new migration every time it runs
        var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        builder.HasData(
            new Category { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "General Practice", IconUrl = "general-practice.png", CreatedAt = seedDate },
            new Category { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Cardiology", IconUrl = "cardiology.png", CreatedAt = seedDate },
            new Category { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Pediatrics", IconUrl = "pediatrics.png", CreatedAt = seedDate },
            new Category { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Neurology", IconUrl = "neurology.png", CreatedAt = seedDate },
            new Category { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Dermatology", IconUrl = "dermatology.png", CreatedAt = seedDate },
            new Category { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Name = "Psychiatry", IconUrl = "psychiatry.png", CreatedAt = seedDate },
            new Category { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), Name = "Orthopedics", IconUrl = "orthopedics.png", CreatedAt = seedDate },
            new Category { Id = Guid.Parse("88888888-8888-8888-8888-888888888888"), Name = "Dentistry", IconUrl = "dentistry.png", CreatedAt = seedDate },
            new Category { Id = Guid.Parse("99999999-9999-9999-9999-999999999999"), Name = "Ophthalmology", IconUrl = "ophthalmology.png", CreatedAt = seedDate }
        );
    }
}
