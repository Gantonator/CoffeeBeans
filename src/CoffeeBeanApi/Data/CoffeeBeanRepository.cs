using CoffeeBeanApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBeanApi.Data;

public interface ICoffeeBeanRepository
{
    Task<IEnumerable<CoffeeBean>> GetAll(CancellationToken cancellationToken = default);
    Task<CoffeeBean?> GetById(int id, CancellationToken cancellationToken = default);
}

public class CoffeeBeanRepository : ICoffeeBeanRepository
{
    private readonly CoffeeBeanContext _context;

    public CoffeeBeanRepository(CoffeeBeanContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CoffeeBean>> GetAll(CancellationToken cancellationToken = default)
    {
        return await _context.CoffeeBeans
            .Include(b => b.Country)
            .Include(b => b.Colour)
            .ToListAsync(cancellationToken);
    }

    public async Task<CoffeeBean?> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await _context.CoffeeBeans
            .Include(b => b.Country)
            .Include(b => b.Colour)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }
}