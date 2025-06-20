using CoffeeBeanApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBeanApi.Data;

public interface ICountryRepository
{
    Task<IEnumerable<Country>> GetAll(CancellationToken cancellationToken = default);
    Task<Country?> GetById(int id, CancellationToken cancellationToken = default);
    Task<Country> Create(Country country, CancellationToken cancellationToken = default);
    Task<bool> CountryExists(int countryId, CancellationToken cancellationToken = default);
}

public class CountryRepository : ICountryRepository
{
    private readonly CoffeeBeanContext _context;

    public CountryRepository(CoffeeBeanContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Country>> GetAll(CancellationToken cancellationToken = default)
    {
        return await _context.Countries.ToListAsync(cancellationToken);
    }

    public async Task<Country?> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Countries.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<Country> Create(Country input, CancellationToken cancellationToken = default)
    {
        _context.Countries.Add(input);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetById(input.Id, cancellationToken) ?? input;
    }
    
    public async Task<bool> CountryExists(int countryId, CancellationToken cancellationToken = default)
    {
        return await _context.Countries.AnyAsync(c => c.Id == countryId, cancellationToken);
    }
}