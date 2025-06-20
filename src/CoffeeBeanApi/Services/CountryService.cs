using Microsoft.EntityFrameworkCore;
using CoffeeBeanApi.Data;
using CoffeeBeanApi.Models;

namespace CoffeeBeanApi.Services;

public interface ICountryService
{
    Task<IEnumerable<Country>> GetAll(CancellationToken cancellationToken = default);
    Task<Country?> GetById(int id, CancellationToken cancellationToken = default);
    Task<Country?> Create(Country country, CancellationToken cancellationToken = default);
}

public class CountryService : ICountryService
{
    private readonly ICountryRepository _repository;

    public CountryService(ICountryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Country>> GetAll(CancellationToken cancellationToken = default)
    {
        var beans = await _repository.GetAll(cancellationToken);
        return beans;
    }

    public async Task<Country?> GetById(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return null;
        }

        return await _repository.GetById(id, cancellationToken);
    }

    public async Task<Country?> Create(Country input, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input.Name))
        {
            return null;
        }

        return await _repository.Create(input, cancellationToken);
    }
}