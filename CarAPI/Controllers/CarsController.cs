
using CarAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarAPI.Controllers
{
    [Route("api/cars")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        public CarsController(ILogger<CarsController> logger, CarContext context)
        {
            _logger = logger;
            _context = context;
        }

        private readonly CarContext _context;
        private readonly ILogger<CarsController> _logger;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> GetCars()
        {
            return await _context.Cars.ToListAsync();
        }

        [HttpGet("{vin}")]
        public async Task<ActionResult<Car>> GetCar(string vin)
        {
            var car = await _context.Cars.FindAsync(vin);

            if (car == null)
            {
                return NotFound();
            }

            return car;
        }

        [HttpPut("{vin}")]
        public async Task<IActionResult> PutCar(string vin, Car car)
        {
            if (!VinValidator.ValidateVin(car.Vin))
            {
                return BadRequest("Invalid VIN.");
            }

            if (vin != car.Vin)
            {
                return BadRequest("Mismatch between supplied VIN and VIN of car.");
            }

            _context.Entry(car).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Car>> PostCar(Car car)
        {
            if (!VinValidator.ValidateVin(car.Vin))
            {
                return BadRequest("Invalid VIN.");
            }

            if (await CarExists(car.Vin))
            {
                return BadRequest("Car already exists in DB.");
            }

            _logger.LogInformation("adding Car: [{Car}]", car);

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            var action = CreatedAtAction(nameof(GetCar), new { vin = car.Vin }, car);
            return action;
        }

        [HttpDelete("{vin}")]
        public async Task<IActionResult> DeleteCar(string vin)
        {
            if (!VinValidator.ValidateVin(vin))
            {
                return BadRequest("Invalid VIN.");
            }

            var car = await _context.Cars.FindAsync(vin);
            if (car == null)
            {
                return NotFound();
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> CarExists(string vin)
        {
            return await _context.Cars.AnyAsync(e => e.Vin == vin);
        }
    }
}
