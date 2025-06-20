using CoffeeBeanApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBeanApi.Data;

public interface IColourRepository
{
    Task<IEnumerable<Colour>> GetAll(CancellationToken cancellationToken = default);
    Task<Colour?> GetById(int id, CancellationToken cancellationToken = default);
    Task<Colour> Create(Colour colour, CancellationToken cancellationToken = default);
    Task<bool> ColourExists(int colourId, CancellationToken cancellationToken = default);
}

public class ColourRepository : IColourRepository
{
    private readonly CoffeeBeanContext _context;

    public ColourRepository(CoffeeBeanContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Colour>> GetAll(CancellationToken cancellationToken = default)
    {
        return await _context.Colours.ToListAsync(cancellationToken);
    }

    public async Task<Colour?> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Colours.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<Colour> Create(Colour input, CancellationToken cancellationToken = default)
    {
        _context.Colours.Add(input);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetById(input.Id, cancellationToken) ?? input;
    }
    
    public async Task<bool> ColourExists(int colourId, CancellationToken cancellationToken = default)
    {
        return await _context.Colours.AnyAsync(c => c.Id == colourId, cancellationToken);
    }
}