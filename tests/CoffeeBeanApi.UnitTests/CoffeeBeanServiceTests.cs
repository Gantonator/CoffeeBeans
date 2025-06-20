using CoffeeBeanApi.Data;
using CoffeeBeanApi.Models;
using CoffeeBeanApi.Services;
using Moq;

namespace CoffeeBeanApi.UnitTests;

public class CoffeeBeanServiceTests
{
    private readonly Mock<ICoffeeBeanRepository> _mockRepository;
    private readonly Mock<IColourRepository> _mockColourRepository;
    private readonly Mock<ICountryRepository> _mockCountryRepository;
    private readonly CoffeeBeanService _service;

    public CoffeeBeanServiceTests()
    {
        _mockRepository = new Mock<ICoffeeBeanRepository>();
        _mockColourRepository = new Mock<IColourRepository>();
        _mockCountryRepository = new Mock<ICountryRepository>();
        _service = new CoffeeBeanService(_mockRepository.Object, _mockColourRepository.Object, _mockCountryRepository.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOrderedCoffeeBeans()
    {
        var coffeeBeans = new List<CoffeeBean>
        {
            new CoffeeBean { Id = 1, Name = "Silly bean", Description = "A silly bean"},
            new CoffeeBean { Id = 2, Name = "Alpha bean", Description = "A bean" },
            new CoffeeBean { Id = 3, Name = "Big bean", Description = "A bean" }
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
    public async Task GetById_WithValidId_ReturnsBean()
    {
        var expectedBean = new CoffeeBean { Id = 1, Name = "Bean" , Description = "A silly bean" };
        _mockRepository.Setup(r => r.GetById(expectedBean.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBean);

        var result = await _service.GetById(expectedBean.Id);

        Assert.NotNull(result);
        Assert.Equal(expectedBean.Id, result.Id);
        Assert.Equal(expectedBean.Name, result.Name);

        _mockRepository.Verify(r => r.GetById(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNull()
    {
        var result = await _service.GetById(0);

        Assert.Null(result);

        _mockRepository.Verify(r => r.GetById(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Create_WithValidBean_ReturnsCreatedBean()
    {
        var newBean = new CoffeeBeanCreateInput
        {
            Name = "Test Bean",
            Description = "Test Description",
            CountryId = 1,
            ColourId = 1,
            Cost = 10
        };

        var createdBean = new CoffeeBean
        {
            Id = 1,
            Name = "Test Bean",
            Description = "Test Description",
            CountryId = 1,
            ColourId = 1,
            Cost = 10
        };

        _mockCountryRepository.Setup(r => r.CountryExists(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockColourRepository.Setup(r => r.ColourExists(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockRepository.Setup(r => r.Create(newBean, It.IsAny<CancellationToken>())).ReturnsAsync(createdBean);

        var result = await _service.Create(newBean);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Bean", result.Name);
        _mockRepository.Verify(r => r.Create(newBean, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_WithInvalidCountry_ReturnsNull()
    {
        var newBean = new CoffeeBeanCreateInput
        {
            Name = "Test Bean",
            Description = "Test Description",
            CountryId = 999,
            ColourId = 1,
            Cost = 10.99m
        };

        _mockCountryRepository.Setup(r => r.CountryExists(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _mockColourRepository.Setup(r => r.ColourExists(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _service.Create(newBean);

        Assert.Null(result);
        _mockRepository.Verify(r => r.Create(It.IsAny<CoffeeBeanCreateInput>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Create_WithEmptyName_ReturnsNull()
    {
        var newBean = new CoffeeBeanCreateInput
        {
            Name = "",
            Description = "Test Description",
            CountryId = 1,
            ColourId = 1,
            Cost = 10.99m
        };

        var result = await _service.Create(newBean);

        Assert.Null(result);
        _mockRepository.Verify(r => r.Create(It.IsAny<CoffeeBeanCreateInput>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Update_WithValidBean_ReturnsUpdatedBean()
    {
        var id = 1;
        var updateBean = new CoffeeBeanUpdateInput
        {
            Name = "Updated Bean",
            Description = "Updated Description",
            CountryId = 1,
            ColourId = 1,
            Cost = 15.99m
        };
        var returnBean = new CoffeeBean
        {
            Id = 1,
            Name = "Updated Bean",
            Description = "Updated Description",
            CountryId = 1,
            ColourId = 1,
            Cost = 15.99m
        };

        _mockCountryRepository.Setup(r => r.CountryExists(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockColourRepository.Setup(r => r.ColourExists(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockRepository.Setup(r => r.Update(id,updateBean, It.IsAny<CancellationToken>())).ReturnsAsync(returnBean);

        var result = await _service.Update(id, updateBean);

        Assert.NotNull(result);
        Assert.Equal(updateBean.Name, result.Name);
        _mockRepository.Verify(r => r.Update(id,updateBean, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_WithInvalidId_ReturnsNull()
    {
        var updateBean = new CoffeeBeanUpdateInput
        {
            Name = "Updated Bean",
            Description = "Updated Description"
        };

        var result = await _service.Update(0, updateBean);

        Assert.Null(result);
        _mockRepository.Verify(r => r.Update(It.IsAny<int>() ,It.IsAny<CoffeeBeanUpdateInput>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Delete_WithValidId_ReturnsTrue()
    {
        _mockRepository.Setup(r => r.Delete(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _service.Delete(1);

        Assert.True(result);
        _mockRepository.Verify(r => r.Delete(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_WithInvalidId_ReturnsFalse()
    {
        var result = await _service.Delete(0);

        Assert.False(result);
        _mockRepository.Verify(r => r.Delete(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public void OrderCoffeeBeans_WithSingleBean_ReturnsSingleBean()
    {
        var beans = new List<CoffeeBean>
        {
            new CoffeeBean { Name = "Some Bean", Description = "A bean" }
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
            new CoffeeBean { Name = "Z", Description = "A bean" },
            new CoffeeBean { Name = "A", Description = "A bean" },
            new CoffeeBean { Name = "C", Description = "A bean" },
            new CoffeeBean { Name = "B", Description = "A bean" }
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