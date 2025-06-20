using AutoMapper;
using CoffeeBeanApi.Models;
using CoffeeBeanApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryService _countryService;

        public CountriesController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Country>>> GetAll(CancellationToken cancellationToken)
        {
            var beans = await _countryService.GetAll(cancellationToken);
            return Ok(beans);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Country>> GetById(int id, CancellationToken cancellationToken)
        {
            var country = await _countryService.GetById(id, cancellationToken);

            if (country == null)
            {
                return NotFound();
            }

            return Ok(country);
        }

        [HttpPost]
        public async Task<ActionResult<Country>> Create([FromBody] Country input, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdCountry = await _countryService.Create(input, cancellationToken);

            if (createdCountry == null)
            {
                return BadRequest("Country creation failed: name was probably invalid!");
            }

            return CreatedAtAction(nameof(GetById), new { id = createdCountry.Id }, createdCountry);
        }

    }
}