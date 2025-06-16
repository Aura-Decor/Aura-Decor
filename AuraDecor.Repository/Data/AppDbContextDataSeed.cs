using AuraDecor.Core.Entities;
using Newtonsoft.Json;

namespace AuraDecor.Repository.Data;

public static class AppDbContextDataSeed
{
    
  public  static async Task SeedAsync(AppDbContext context)
  {
    // Seed Brands first
    if (!context.FurnitureBrands.Any())
    {
      var brandsData = await File.ReadAllTextAsync("../AuraDecor.Repository/Data/DataSeed/brands.json");
      var brands = JsonConvert.DeserializeObject<List<FurnitureBrand>>(brandsData);
      if (brands is not null && brands.Any())
      {
        await context.FurnitureBrands.AddRangeAsync(brands);
        await context.SaveChangesAsync();
      }
    }

    // Seed Categories
    if (!context.FurnitureCategories.Any())
    {
      var categoriesData = await File.ReadAllTextAsync("../AuraDecor.Repository/Data/DataSeed/categories.json");
      var categories = JsonConvert.DeserializeObject<List<FurnitureCategory>>(categoriesData);
      if (categories is not null && categories.Any())
      {
        await context.FurnitureCategories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
      }
    }

    // Seed Style Types
    if (!context.StyleTypes.Any())
    {
      var styleTypesData = await File.ReadAllTextAsync("../AuraDecor.Repository/Data/DataSeed/styletypes.json");
      var styleTypes = JsonConvert.DeserializeObject<List<StyleType>>(styleTypesData);
      if (styleTypes is not null && styleTypes.Any())
      {
        await context.StyleTypes.AddRangeAsync(styleTypes);
        await context.SaveChangesAsync();
      }
    }

    // Seed Colors
    if (!context.Colors.Any())
    {
      var colorsData = await File.ReadAllTextAsync("../AuraDecor.Repository/Data/DataSeed/colors.json");
      var colors = JsonConvert.DeserializeObject<List<Color>>(colorsData);
      if (colors is not null && colors.Any())
      {
        await context.Colors.AddRangeAsync(colors);
        await context.SaveChangesAsync();
      }
    }

    // Seed Furniture last (depends on other entities)
    if (!context.Furniture.Any())
    {
      var furnitureData = await File.ReadAllTextAsync("../AuraDecor.Repository/Data/DataSeed/Furniture.json");
      var furniture = JsonConvert.DeserializeObject<List<Furniture>>(furnitureData);
      if (furniture is not null && furniture.Any())
      {
        await context.Furniture.AddRangeAsync(furniture);
        await context.SaveChangesAsync();
      }
    }
  }
}
