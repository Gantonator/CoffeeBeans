using CoffeeBeanApi.Data;
using CoffeeBeanApi.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;

namespace CoffeeBeanApi.IntegrationTests;

public class CoffeeBeanApiIntegrationTests : IClassFixture<TestWebApplicationFactory<Program>>, IDisposable
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public CoffeeBeanApiIntegrationTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        SeedTestData();
    }

    private void SeedTestData()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CoffeeBeanContext>();

        context.CoffeeBeans.RemoveRange(context.CoffeeBeans);
        context.Countries.RemoveRange(context.Countries);
        context.Colours.RemoveRange(context.Colours);
        context.SaveChanges();

        context.Countries.AddRange(new Country { Id = 1, Name = "England" }, new Country { Id = 2, Name = "Scotland" }, new Country { Id = 3, Name = "Colombia" });

        context.Colours.AddRange(new Colour { Id = 1, Name = "Red" }, new Colour { Id = 2, Name = "Green" }, new Colour { Id = 3, Name = "Yellow" });

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

        context.CoffeeBeans.AddRange(coffeeBeans);
        context.SaveChanges();
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

        List<CoffeeBean> orderedBeansFromContext;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CoffeeBeanContext>();
            orderedBeansFromContext = context.CoffeeBeans.OrderBy(x => x.Name).ToList();
        }

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

    [Fact]
    public async Task GetCoffeeBeanById_WithValidId_ReturnsBean()
    {
        CoffeeBean beanFromContext;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CoffeeBeanContext>();
            beanFromContext = context.CoffeeBeans.Last();
        }

        var response = await _client.GetAsync($"/api/coffeebeans/{beanFromContext.Id}");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var bean = JsonSerializer.Deserialize<CoffeeBean>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(bean);
        Assert.Equal(beanFromContext.Id, bean.Id);
        Assert.Equal(beanFromContext.Name, bean.Name);
    }

    [Fact]
    public async Task GetCoffeeBeanById_WithInvalidId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/coffeebeans/999");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateCoffeeBean_WithValidData_ReturnsCreatedBean()
    {
        var input = new CoffeeBeanCreateInput()
        {
            Name = "New Test Bean",
            Description = "A delicious new test bean",
            CountryId = 1,
            ColourId = 1,
            Cost = 10,
            Image = "https://images.unsplash.com/photo-fake-test-photo.jpg"
        };

        var response = await _client.PostAsJsonAsync("/api/coffeebeans", input);

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var bean = JsonSerializer.Deserialize<CoffeeBean>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(bean);
        Assert.Equal(input.Name, bean.Name);
        Assert.Equal(input.Description, bean.Description);
        Assert.Equal(input.Cost, bean.Cost);
        Assert.True(bean.Id > 0);
    }

    [Fact]
    public async Task CreateCoffeeBean_WithInvalidCountry_ReturnsBadRequest()
    {
        var request = new CoffeeBeanCreateInput()
        {
            Name = "Invalid Bean",
            Description = "Bean with invalid country",
            CountryId = 999,
            ColourId = 1,
            Cost = 10
        };

        var response = await _client.PostAsJsonAsync("/api/coffeebeans", request);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateCoffeeBean_WithValidData_ReturnsUpdatedBean()
    {
        CoffeeBean existingBean;

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CoffeeBeanContext>();
            existingBean = context.CoffeeBeans.Last();
        }

        var input = new CoffeeBeanUpdateInput
        {
            Name = "Updated Bean Name",
            Description = "Updated description",
            CountryId = 2,
            ColourId = 2,
            Cost = 10,
            Image = "https://images.unsplash.com/photo-fake-test-photo.jpg"
        };

        var response = await _client.PutAsJsonAsync($"/api/coffeebeans/{existingBean.Id}", input);

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var bean = JsonSerializer.Deserialize<CoffeeBean>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(bean);
        Assert.Equal(input.Name, bean.Name);
        Assert.Equal(input.Description, bean.Description);
        Assert.Equal(input.Cost, bean.Cost);
        Assert.True(bean.IsBeanOfTheDay);
    }

    [Fact]
    public async Task UpdateCoffeeBean_WithInvalidId_ReturnsNotFound()
    {
        var request = new CoffeeBeanUpdateInput
        {
            Name = "Updated Bean",
            Description = "Updated description",
            CountryId = 1,
            ColourId = 1,
            Cost = 10
        };

        var response = await _client.PutAsJsonAsync("/api/coffeebeans/999", request);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCoffeeBean_WithValidId_ReturnsNoContent()
    {
        CoffeeBean existingBean;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CoffeeBeanContext>();
            existingBean = context.CoffeeBeans.First();
        }

        var response = await _client.DeleteAsync($"/api/coffeebeans/{existingBean.Id}");

        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        CoffeeBean? deletedBean;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CoffeeBeanContext>();
            deletedBean = await context.CoffeeBeans.FindAsync(existingBean.Id);
        }

        Assert.Null(deletedBean);
    }

    [Fact]
    public async Task DeleteCoffeeBean_WithInvalidId_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/coffeebeans/999");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}