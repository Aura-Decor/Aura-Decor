using AuraDecor.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuraDecor.Repository.Data.Config
{
    public class CartConfig : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        { builder.Property(c => c.UserId)
                .IsRequired();

            builder.HasOne<User>() 
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(c => c.UserId);
        }
    }

    public class CartItemConfig : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.Property(ci => ci.Quantity)
                .IsRequired()
                .HasDefaultValue(1)
                .HasAnnotation("MinValue", 1);

            builder.HasOne(ci => ci.Cart)
                .WithMany()
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasOne(ci => ci.Furniture)
                .WithMany()
                .HasForeignKey(ci => ci.FurnitureId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.HasIndex(ci => new { ci.CartId, ci.FurnitureId })
                .IsUnique();

        }
    }
}