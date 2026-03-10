using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using task.Entities;

namespace task.Data;

internal sealed class OfficeConfiguration : IEntityTypeConfiguration<Office>
{
    public void Configure(EntityTypeBuilder<Office> builder)
    {
        builder.ToTable("offices");

        builder.HasKey(x => x.Id);

        builder.HasMany(o => o.Phones)
               .WithOne(p => p.Office)
               .HasForeignKey(p => p.OfficeId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => o.CityCode);

        builder.OwnsOne(o => o.Coordinates);

        builder.Property(o => o.Code);
        builder.Property(o => o.CityCode).IsRequired();
        builder.Property(o => o.Uuid);
        builder.Property(o => o.Type);
        builder.Property(o => o.CountryCode);
        builder.Property(o => o.AddressRegion);
        builder.Property(o => o.AddressCity);
        builder.Property(o => o.AddressStreet);
        builder.Property(o => o.AddressHouseNumber);
        builder.Property(o => o.AddressApartment);
        builder.Property(o => o.WorkTime);
    }
}
