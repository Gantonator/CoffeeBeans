using AutoMapper;
using CoffeeBeanApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeBeanApi.Data;

public interface ICoffeeBeanRepository
{
    Task<IEnumerable<CoffeeBean>> GetAll(CancellationToken cancellationToken = default);
    Task<CoffeeBean?> GetById(int id, CancellationToken cancellationToken = default);
    Task<CoffeeBean> Create(CoffeeBeanCreateInput coffeeBean, CancellationToken cancellationToken = default);
    Task<CoffeeBean?> Update(int id, CoffeeBeanUpdateInput input, CancellationToken cancellationToken = default);
    Task<bool> Delete(int id, CancellationToken cancellationToken = default);
    Task<bool> CountryExists(int countryId, CancellationToken cancellationToken = default);
    Task<bool> ColourExists(int colourId, CancellationToken cancellationToken = default);
}

public class CoffeeBeanRepository : ICoffeeBeanRepository
{
    private readonly CoffeeBeanContext _context;
    private readonly IMapper _mapper;

    public CoffeeBeanRepository(CoffeeBeanContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
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

    public async Task<CoffeeBean> Create(CoffeeBeanCreateInput input, CancellationToken cancellationToken = default)
    {
        var coffeeBean = _mapper.Map<CoffeeBean>(input);
        _context.CoffeeBeans.Add(coffeeBean);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetById(coffeeBean.Id, cancellationToken) ?? coffeeBean;
    }

    public async Task<CoffeeBean?> Update(int id, CoffeeBeanUpdateInput input,
        CancellationToken cancellationToken = default)
    {
        var existingBean = await _context.CoffeeBeans.FindAsync(id);
        if (existingBean == null)
        {
            return null;
        }

        _mapper.Map(input, existingBean);

        await _context.SaveChangesAsync(cancellationToken);

        return await GetById(id, cancellationToken);
    }

    public async Task<bool> Delete(int id, CancellationToken cancellationToken = default)
    {
        var coffeeBean = await _context.CoffeeBeans.FindAsync(id);
        if (coffeeBean == null)
        {
            return false;
        }

        _context.CoffeeBeans.Remove(coffeeBean);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> CountryExists(int countryId, CancellationToken cancellationToken = default)
    {
        return await _context.Countries.AnyAsync(c => c.Id == countryId, cancellationToken);
    }

    public async Task<bool> ColourExists(int colourId, CancellationToken cancellationToken = default)
    {
        return await _context.Colours.AnyAsync(c => c.Id == colourId, cancellationToken);
    }
}