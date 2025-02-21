using AuraDecor.Core.Entities;
using Newtonsoft.Json;

namespace AuraDecor.Repository.Data;

public static class AppDbContextDataSeed
{
    
  public  static async Task SeedAsync(AppDbContext context)
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
    // if (!context.FurnitureBrands.Any())
    // {
    //   var furnitureData = await File.ReadAllTextAsync("../AuraDecor.Repository/Data/DataSeed/brands.json");
    //   var furniture = JsonConvert.DeserializeObject<List<FurnitureBrand>>(furnitureData);
    //   if (furniture is not null && furniture.Any())
    //   {
    //     await context.FurnitureBrands.AddRangeAsync(furniture);
    //     await context.SaveChangesAsync();
    //   }
    // }
    // if (!context.FurnitureCategories.Any())
    // {
    //   var furnitureData = await File.ReadAllTextAsync("../AuraDecor.Repository/Data/DataSeed/categories.json");
    //   var furniture = JsonConvert.DeserializeObject<List<FurnitureCategory>>(furnitureData);
    //   if (furniture is not null && furniture.Any())
    //   {
    //     await context.FurnitureCategories.AddRangeAsync(furniture);
    //     await context.SaveChangesAsync();
    //   }
    // }
  }
}
