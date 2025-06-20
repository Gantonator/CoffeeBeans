using CoffeeBeanApi.Data;
using CoffeeBeanApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Json;
using System.Text.Json;

namespace CoffeeBeanApi.IntegrationTests;

public class CoffeeBeanApiIntegrationTests : IClassFixture<TestWebApplicationFactory<Program>>, IDisposable
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly CoffeeBeanContext _context;

    public CoffeeBeanApiIntegrationTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _scope = _factory.Services.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<CoffeeBeanContext>();
        SeedTestData();
    }

    private void SeedTestData()
    {
        _context.CoffeeBeans.RemoveRange(_context.CoffeeBeans);
        _context.Countries.RemoveRange(_context.Countries);
        _context.Colours.RemoveRange(_context.Colours);
        _context.SaveChanges();

        _context.Countries.AddRange(new Country { Id = 1, Name = "England" }, new Country { Id = 2, Name = "Scotland" }, new Country { Id = 3, Name = "Colombia" });

        _context.Colours.AddRange(new Colour { Id = 1, Name = "Red" }, new Colour { Id = 2, Name = "Green" }, new Colour { Id = 3, Name = "Yellow" });

        var coffeeBeans = new List<CoffeeBean>
        {
            new CoffeeBean
            {
                Id = 1,
                Name = "Red bean",
                Description = "A red bean from England",
                CountryId = 1,
                ColourId = 1,
                Cost = 10,
                IsBeanOfTheDay = true,
                Image = "Something.jpg"
            },
            new CoffeeBean
            {
                Id = 2,
                Name = "Green Scotch bean",
                Description = "A green bean from Scotland",
                CountryId = 2,
                ColourId = 2,
                Cost = 20.50M,
                IsBeanOfTheDay = false,
                Image = "Something.jpg"
            },
            new CoffeeBean
            {
                Id = 3,
                Name = "Yellow Colombian bean",
                Description = "Nice and yellow",
                CountryId = 3,
                ColourId = 3,
                Cost = 1,
                IsBeanOfTheDay = false,
                Image = "Something.jpg"
            },
            new CoffeeBean
            {
                Id = 4,
                Name = "English mustard bean",
                Description = "English and yellow",
                CountryId = 1,
                ColourId = 3,
                Cost = 4.99M,
                IsBeanOfTheDay = false,
                Image = "Something.jpg"
            }
        };

        _context.CoffeeBeans.AddRange(coffeeBeans);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllCoffeeBeans_ReturnsSuccessAndOrderedBeans()
    {
        var response = await _client.GetAsync("/api/coffeebeans");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var beans = JsonSerializer.Deserialize<List<CoffeeBean>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var orderedBeansFromContext = _context.CoffeeBeans.OrderBy(x => x.Name).ToList();

        Assert.NotNull(beans);
        Assert.Equal(beans.Count, orderedBeansFromContext.Count);

        for (var index = 0; index < beans.Count; index++)
        {
            var bean = beans[index];
            var beanFromContext = orderedBeansFromContext[index];
            Assert.Equal(bean.Name, beanFromContext.Name);

            Assert.NotNull(bean.Country);
            Assert.NotNull(bean.Colour);
        }
    }

    public void Dispose()
    {
        _scope?.Dispose();
        _client?.Dispose();
    }
}