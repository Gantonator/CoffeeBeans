using CoffeeBeanApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoffeeBeansController : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoffeeBean>>> GetAll(CancellationToken cancellationToken)
        {
            var beans = new List<CoffeeBean>{new CoffeeBean
                {
                    Id = 0,
                    Name = "TestBean",
                    Description = "A really lovely bean",
                    CountryId = 0,
                    ColourId = 0,
                    Country = new Country
                    {
                        Name = "Beanville"
                    },
                    Colour = new Colour
                    {
                        Name = "Green"
                    },
                    Cost = 0,
                    Image = null,
                    IsBeanOfTheDay = false,
                    CreatedAt = default
                }
            };
            return Ok(beans);
        }
    }
}
