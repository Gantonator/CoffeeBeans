using CoffeeBeanApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Text.Json;

namespace CoffeeBeanApi.Data
{
    public static class DataSeeder
    {
        public static async Task SeedDataAsync(CoffeeBeanContext context, IWebHostEnvironment environment)
        {
            if (await context.CoffeeBeans.AnyAsync())
                return;

            var jsonFilePath = Path.Combine(environment.ContentRootPath, "Data", "AllTheBeans.json");
            var jsonData = await File.ReadAllTextAsync(jsonFilePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var jsonBeans = JsonSerializer.Deserialize<JsonElement[]>(jsonData, options);

            var uniqueCountries = jsonBeans
                .Select(j => j.GetProperty("Country").GetString()?.Trim())
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .ToList();

            var uniqueColours = jsonBeans
                .Select(j => j.GetProperty("colour").GetString()?.Trim())
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .ToList();

            var countries = uniqueCountries.Select(name => new Country { Name = name! }).ToList();
            context.Countries.AddRange(countries);
            await context.SaveChangesAsync();

            var colours = uniqueColours.Select(name => new Colour { Name = name! }).ToList();
            context.Colours.AddRange(colours);
            await context.SaveChangesAsync();

            var countryDictionary = countries.ToDictionary(c => c.Name, c => c.Id);
            var colourLookup = colours.ToDictionary(c => c.Name, c => c.Id);

            // Create CoffeeBean entities with foreign key references
            var beans = new List<CoffeeBean>();
            foreach (var jsonBean in jsonBeans)
            {
                var costString = jsonBean.GetProperty("Cost").GetString()?.Replace("£", "") ?? "0";
                var countryName = jsonBean.GetProperty("Country").GetString()?.Trim() ?? "";
                var colourName = jsonBean.GetProperty("colour").GetString()?.Trim() ?? "";

                // Skip beans with missing required data
                if (string.IsNullOrEmpty(countryName) || string.IsNullOrEmpty(colourName))
                {
                    continue;
                }

                beans.Add(new CoffeeBean
                {
                    OriginalId = jsonBean.GetProperty("_id").GetString() ?? "",
                    Name = jsonBean.GetProperty("Name").GetString() ?? "",
                    Description = jsonBean.GetProperty("Description").GetString()?.Trim() ?? "",
                    CountryId = countryDictionary[countryName],
                    ColourId = colourLookup[colourName],
                    Cost = decimal.TryParse(costString, out var cost) ? cost : 0,
                    Image = jsonBean.GetProperty("Image").GetString(),
                    IsBeanOfTheDay = jsonBean.GetProperty("isBOTD").GetBoolean()
                });
            }

            context.CoffeeBeans.AddRange(beans);
            await context.SaveChangesAsync();
        }
    }
}