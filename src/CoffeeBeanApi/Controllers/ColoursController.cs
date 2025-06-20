using AutoMapper;
using CoffeeBeanApi.Models;
using CoffeeBeanApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeBeanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColoursController : ControllerBase
    {
        private readonly IColourService _colourService;

        public ColoursController(IColourService colourService)
        {
            _colourService = colourService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Colour>>> GetAll(CancellationToken cancellationToken)
        {
            var beans = await _colourService.GetAll(cancellationToken);
            return Ok(beans);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Colour>> GetById(int id, CancellationToken cancellationToken)
        {
            var colour = await _colourService.GetById(id, cancellationToken);

            if (colour == null)
            {
                return NotFound();
            }

            return Ok(colour);
        }

        [HttpPost]
        public async Task<ActionResult<Colour>> Create([FromBody] Colour input, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdColour = await _colourService.Create(input, cancellationToken);

            if (createdColour == null)
            {
                return BadRequest("Colour creation failed: Name was probably invalid.");
            }

            return CreatedAtAction(nameof(GetById), new { id = createdColour.Id }, createdColour);
        }

    }
}