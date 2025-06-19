using CoffeeBeanApi.Models;
using CoffeeBeanApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoffeeBeansController : ControllerBase
    {
        private readonly ICoffeeBeanService _coffeeBeanService;

        public CoffeeBeansController(ICoffeeBeanService coffeeBeanService)
        {
            _coffeeBeanService = coffeeBeanService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoffeeBean>>> GetAll(CancellationToken cancellationToken)
        {
            var beans = await _coffeeBeanService.GetAll(cancellationToken);
            return Ok(beans);
        }
    }
}
