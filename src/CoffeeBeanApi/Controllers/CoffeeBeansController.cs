using AutoMapper;
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

        [HttpGet("{id}")]
        public async Task<ActionResult<CoffeeBean>> GetById(int id, CancellationToken cancellationToken)
        {
            var bean = await _coffeeBeanService.GetById(id, cancellationToken);

            if (bean == null)
            {
                return NotFound();
            }

            return Ok(bean);
        }

        [HttpPost]
        public async Task<ActionResult<CoffeeBean>> Create([FromBody] CoffeeBeanCreateInput input,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdBean = await _coffeeBeanService.Create(input, cancellationToken);

            if (createdBean == null)
            {
                return BadRequest("Invalid country or colour specified");
            }

            return CreatedAtAction(nameof(GetById), new { id = createdBean.Id }, createdBean);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CoffeeBean>> Update(int id, [FromBody] CoffeeBeanUpdateInput input,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var updatedBean = await _coffeeBeanService.Update(id, input, cancellationToken);

            if (updatedBean == null)
            {
                return NotFound();
            }

            return Ok(updatedBean);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var success = await _coffeeBeanService.Delete(id, cancellationToken);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}