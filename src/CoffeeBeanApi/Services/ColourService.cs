using Microsoft.EntityFrameworkCore;
using CoffeeBeanApi.Data;
using CoffeeBeanApi.Models;

namespace CoffeeBeanApi.Services;

public interface IColourService
{
    Task<IEnumerable<Colour>> GetAll(CancellationToken cancellationToken = default);
    Task<Colour?> GetById(int id, CancellationToken cancellationToken = default);
    Task<Colour?> Create(Colour colour, CancellationToken cancellationToken = default);
}

public class ColourService : IColourService
{
    private readonly IColourRepository _repository;

    public ColourService(IColourRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Colour>> GetAll(CancellationToken cancellationToken = default)
    {
        var beans = await _repository.GetAll(cancellationToken);
        return beans;
    }

    public async Task<Colour?> GetById(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return null;
        }

        return await _repository.GetById(id, cancellationToken);
    }

    public async Task<Colour?> Create(Colour input, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input.Name))
        {
            return null;
        }

        return await _repository.Create(input, cancellationToken);
    }
}