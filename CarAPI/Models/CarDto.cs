using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CarAPI.Models
{
    public class CarDto
    {
        public CarDto(string make, int mileage, string model)
        {
            CreatedAt = DateTime.Now;
            Make = make;
            Mileage = mileage;
            Model = model;
        }

        public DateTime CreatedAt { get; private set; }

        [Required(AllowEmptyStrings = false)]
        public string Make { get; set; }

        public int Mileage { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Model { get; set; }
    }
}