using CoffeeBeanApi.Data;
using CoffeeBeanApi.Services;
using Moq;

namespace CoffeeBeanApi.UnitTests;

public class CoffeeBeanServiceTests
{
    private readonly Mock<ICoffeeBeanRepository> _mockRepository;
    private readonly CoffeeBeanService _service;

    public CoffeeBeanServiceTests()
    {
        _mockRepository = new Mock<ICoffeeBeanRepository>();
        _service = new CoffeeBeanService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOrderedCoffeeBeans()
    {
        var coffeeBeans = new List<CoffeeBean>
        {
            new CoffeeBean { Id = 1, Name = "Silly bean" },
            new CoffeeBean { Id = 2, Name = "Alpha bean" },
            new CoffeeBean { Id = 3, Name = "Big bean" }
        };

        _mockRepository.Setup(r => r.GetAll(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(coffeeBeans);

        var result = await _service.GetAll();

        var beans = result.ToList();
        Assert.Equal(3, beans.Count);
        Assert.Equal("Alpha bean", beans[0].Name);
        Assert.Equal("Big bean", beans[1].Name);
        Assert.Equal("Silly bean", beans[2].Name);

        _mockRepository.Verify(r => r.GetAll(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void OrderCoffeeBeans_WithSingleBean_ReturnsSingleBean()
    {
        var beans = new List<CoffeeBean>
        {
            new CoffeeBean { Name = "Some Bean" }
        };
        var result = CoffeeBeanService.OrderCoffeeBeans(beans);
        var orderedBeans = result.ToList();
        Assert.Single(orderedBeans);
        Assert.Equal("Some Bean", orderedBeans[0].Name);
    }

    [Fact]
    public void OrderCoffeeBeans_ReturnsBeansOrderedByName()
    {
        var beans = new List<CoffeeBean>
        {
            new CoffeeBean { Name = "Z" },
            new CoffeeBean { Name = "A" },
            new CoffeeBean { Name = "C" },
            new CoffeeBean { Name = "B" }
        };

        var result = CoffeeBeanService.OrderCoffeeBeans(beans);

        var orderedBeans = result.ToList();
        Assert.Equal(4, orderedBeans.Count);
        Assert.Equal("A", orderedBeans[0].Name);
        Assert.Equal("B", orderedBeans[1].Name);
        Assert.Equal("C", orderedBeans[2].Name);
        Assert.Equal("Z", orderedBeans[3].Name);
    }

    [Fact]
    public void OrderCoffeeBeans_WithEmptyList_ReturnsEmptyList()
    {
        var beans = new List<CoffeeBean>();

        var result = CoffeeBeanService.OrderCoffeeBeans(beans);

        Assert.Empty(result);
    }
}