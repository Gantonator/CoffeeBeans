using Microsoft.EntityFrameworkCore;
using CoffeeBeanApi.Data;
using CoffeeBeanApi.Models;

namespace CoffeeBeanApi.Services;

public interface ICoffeeBeanService
{
    Task<IEnumerable<CoffeeBean>> GetAll(CancellationToken cancellationToken = default);
    Task<CoffeeBean?> GetById(int id, CancellationToken cancellationToken = default);
    Task<CoffeeBean?> Create(CoffeeBeanCreateInput coffeeBean, CancellationToken cancellationToken = default);
    Task<CoffeeBean?> Update(int id, CoffeeBeanUpdateInput coffeeBean, CancellationToken cancellationToken = default);
    Task<bool> Delete(int id, CancellationToken cancellationToken = default);
}

public class CoffeeBeanService : ICoffeeBeanService
{
    private readonly ICoffeeBeanRepository _repository;
    private readonly IColourRepository _colourRepository;
    private readonly ICountryRepository _countryRepository;

    public CoffeeBeanService(ICoffeeBeanRepository repository, IColourRepository colourRepository, ICountryRepository countryRepository)
    {
        _repository = repository;
        _colourRepository = colourRepository;
        _countryRepository = countryRepository;
    }

    public async Task<IEnumerable<CoffeeBean>> GetAll(CancellationToken cancellationToken = default)
    {
        var beans = await _repository.GetAll(cancellationToken);
        return OrderCoffeeBeans(beans);
    }

    public async Task<CoffeeBean?> GetById(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return null;
        }

        return await _repository.GetById(id, cancellationToken);
    }

    public async Task<CoffeeBean?> Create(CoffeeBeanCreateInput input, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input.Name) || string.IsNullOrWhiteSpace(input.Description))
        {
            return null;
        }

        var countryExists = await _countryRepository.CountryExists(input.CountryId, cancellationToken);
        var colourExists = await _colourRepository.ColourExists(input.ColourId, cancellationToken);

        if (!countryExists || !colourExists)
        {
            return null;
        }

        return await _repository.Create(input, cancellationToken);
    }

    public async Task<CoffeeBean?> Update(int id, CoffeeBeanUpdateInput input, CancellationToken cancellationToken = default)
    {
        if (id <= 0 || string.IsNullOrWhiteSpace(input.Name) || string.IsNullOrWhiteSpace(input.Description))
        {
            return null;
        }
        
        var countryExists = await _countryRepository.CountryExists(input.CountryId, cancellationToken);
        var colourExists = await _colourRepository.ColourExists(input.ColourId, cancellationToken);

        if (!countryExists || !colourExists)
        {
            return null;
        }

        return await _repository.Update(id, input, cancellationToken);
    }

    public async Task<bool> Delete(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return false;
        }

        return await _repository.Delete(id, cancellationToken);
    }


    internal static IEnumerable<CoffeeBean> OrderCoffeeBeans(IEnumerable<CoffeeBean> beans)
    {
        return beans.OrderBy(b => b.Name);
    }

}