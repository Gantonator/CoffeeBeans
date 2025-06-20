using Microsoft.EntityFrameworkCore;
using CoffeeBeanApi.Data;
using CoffeeBeanApi.Models;

namespace CoffeeBeanApi.Services;

public interface ICoffeeBeanService
{
    Task<IEnumerable<CoffeeBean>> GetAll(CancellationToken cancellationToken = default);
    Task<CoffeeBean?> GetById(int id, CancellationToken cancellationToken = default);
}

public class CoffeeBeanService : ICoffeeBeanService
{
    private readonly ICoffeeBeanRepository _repository;

    public CoffeeBeanService(ICoffeeBeanRepository repository)
    {
        _repository = repository;
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

    internal static IEnumerable<CoffeeBean> OrderCoffeeBeans(IEnumerable<CoffeeBean> beans)
    {
        return beans.OrderBy(b => b.Name);
    }

}