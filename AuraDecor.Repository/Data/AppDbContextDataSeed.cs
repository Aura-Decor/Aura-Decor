using AuraDecor.Core.Entities;
using Newtonsoft.Json;

namespace AuraDecor.Repository.Data;

public static class AppDbContextDataSeed
{
    
  public static async Task SeedAsync(AppDbContext context)
  {
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
    if (!context.FurnitureBrands.Any())
    {
      var furnitureBrandData = await File.ReadAllTextAsync("../AuraDecor.Repository/Data/DataSeed/FurnitureBrand.json");
      var furnitureBrand = JsonConvert.DeserializeObject<List<FurnitureBrand>>(furnitureBrandData);
      if (furnitureBrand is not null && furnitureBrand.Any())
      {
        await context.FurnitureBrands.AddRangeAsync(furnitureBrand);
        await context.SaveChangesAsync();
      }
    }
    if (!context.FurnitureCategories.Any())
    {
      var furnitureCategoryData = await File.ReadAllTextAsync("../AuraDecor.Repository/Data/DataSeed/FurnitureCategory.json");
      var furnitureCategory = JsonConvert.DeserializeObject<List<FurnitureCategory>>(furnitureCategoryData);
      if (furnitureCategory is not null && furnitureCategory.Any())
      {
        await context.FurnitureCategories.AddRangeAsync(furnitureCategory);
        await context.SaveChangesAsync();
      }
    }
  }
}
