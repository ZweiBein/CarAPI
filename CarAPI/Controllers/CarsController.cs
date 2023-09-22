
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using NuGet.Protocol.Core.Types;
using System.ComponentModel.Design;
using Humanizer;

namespace CarAPI.Controllers
{
    [Route("api")]
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

        // [ { "op": "replace", "path": "/make", "value": "yip" } ]

        [HttpPatch("{vin}")]
        public async Task<IActionResult> PatchCar(string vin, [FromBody] JsonPatchDocument<Car> patchDoc)
        {
            if (!ValidateVin(vin))
            {
                return BadRequest();
            }

            if (patchDoc == null)
            {
                return BadRequest("patchDoc object is null");
            }

            for (int i = patchDoc.Operations.Count - 1; i >= 0; i--)
            {
                string pathPropertyName = patchDoc.Operations[i].path.Split("/", StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

                if (!typeof(Car).GetProperties().Where(p => string.Equals(p.Name, pathPropertyName, StringComparison.CurrentCultureIgnoreCase)).Any())
                {
                    return BadRequest();
                }
            }



            var car = await _context.Cars.FindAsync(vin);

            if (car == null)
            {
                return NotFound();
            }

            patchDoc.ApplyTo(car);
            _context.Entry(car).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var action = CreatedAtAction("PatchCar", new { id = car.Id }, car);
            return action;
        }

        [HttpPut("{vin}")]
        public async Task<IActionResult> PutCar(string vin, Car car)
        {
            if (!ValidateVin(car.Id))
            {
                return BadRequest();
            }

            if (vin != car.Id)
            {
                return BadRequest();
            }

            _context.Entry(car).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Car>> PostCar(Car car)
        {
            if (!ValidateVin(car.Id))
            {
                return BadRequest();
            }

            if (CarExists(car.Id))
            {
                return BadRequest();
            }

            _logger.LogInformation("adding Car: [{Car}]", car);

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            var action = CreatedAtAction("PostCar", new { id = car.Id }, car);
            return action;
        }

        [HttpDelete("{vin}")]
        public async Task<IActionResult> DeleteCar(string vin)
        {
            if (!ValidateVin(vin))
            {
                return BadRequest();
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

        private bool CarExists(string id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }

        private static bool ValidateVin(string vin)
        {
            if (vin.Length != 17)
            {
                return false;
            }

            int result;
            int index = 0;
            int checkDigit = 0;
            int checkSum = 0;
            int weight = 0;

            foreach (var c in vin.ToCharArray())
            {
                ++index;
                var character = c.ToString().ToLower();
                if (char.IsNumber(c))
                {
                    result = int.Parse(character);
                }
                else
                {
                    switch (character)
                    {
                        case "a":
                        case "j":
                            result = 1;
                            break;
                        case "b":
                        case "k":
                        case "s":
                            result = 2;
                            break;
                        case "c":
                        case "l":
                        case "t":
                            result = 3;
                            break;
                        case "d":
                        case "m":
                        case "u":
                            result = 4;
                            break;
                        case "e":
                        case "n":
                        case "v":
                            result = 5;
                            break;
                        case "f":
                        case "w":
                            result = 6;
                            break;
                        case "g":
                        case "p":
                        case "x":
                            result = 7;
                            break;
                        case "h":
                        case "y":
                            result = 8;
                            break;
                        case "r":
                        case "z":
                            result = 9;
                            break;
                        default:
                            return false;
                    }
                }

                if (index >= 1 && index <= 7)
                {
                    weight = 9 - index;
                }
                else if (index == 8)
                {
                    weight = 10;
                }
                else if (index >= 10 && index <= 17)
                {
                    weight = 19 - index;
                }
                if (index == 9)
                {
                    weight = 9 - index;
                    checkDigit = character == "x" ? 10 : result;
                }

                checkSum += (result * weight);
            }

            return checkSum % 11 == checkDigit;
        }

    }
}
