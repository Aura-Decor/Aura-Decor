using AuraDecor.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuraDecor.Repository.Data.Config;

public class FurnitureConfig : IEntityTypeConfiguration<Furniture>
{
    public void Configure(EntityTypeBuilder<Furniture> builder)
    {
        builder.Property(F=>F.Name).IsRequired().HasMaxLength(100);
        builder.Property(F=>F.Description).IsRequired().HasMaxLength(180);
        builder.Property(F=>F.Price).HasColumnType("decimal(18,2)");
        builder.Property(F=>F.PictureUrl).IsRequired();
        builder.Property(F=>F.FurnitureModel).IsRequired();
        builder.HasOne(F => F.Brand).WithMany();
        builder.HasOne(F => F.Category)
            .WithMany();

        builder.HasOne(F => F.StyleType).WithMany().HasForeignKey(F => F.StyleTypeId)
         .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(F => F.Color).WithMany().HasForeignKey(F => F.ColorId)
         .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(f => f.HasOffer);
        builder.HasIndex(f => new { f.CategoryId, f.BrandId });
        builder.HasIndex(f => f.Price);
    }
}