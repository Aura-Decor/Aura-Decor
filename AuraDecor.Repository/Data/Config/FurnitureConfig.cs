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
        builder.HasOne(F => F.Brand)
            .WithMany()
            .HasForeignKey(F => F.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(F => F.Category)
            .WithMany()
            .HasForeignKey(F => F.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);;
    }
}