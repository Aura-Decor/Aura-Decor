    using System.Reflection;
    using AuraDecor.Core.Entities;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    namespace AuraDecor.Repository.Data;

    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //
        //     modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        // }
        //
        public DbSet<Furniture> Furniture { get; set; }
        
        public DbSet<FurnitureCategory> FurnitureCategories { get; set; }
        
        public DbSet<FurnitureBrand> FurnitureBrands { get; set; }
    }