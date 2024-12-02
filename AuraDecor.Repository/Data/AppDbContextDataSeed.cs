using AuraDecor.Core.Entities;
using Newtonsoft.Json;

namespace AuraDecor.Repository.Data;

public class AppDbContextDataSeed
{
    
  public async Task SeedAsync(AppDbContext context)
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
  }
}
