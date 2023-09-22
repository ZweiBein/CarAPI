using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CarAPI.Models
{
    public class Car
    {
        public Car(string make, int mileage, string model, string id)
        {
            CreatedAt = DateTime.Now;
            Make = make;
            Mileage = mileage;
            Model = model;
            Id = id;
        }

        public Car(CarDto dto, string id)
        {
            CreatedAt = DateTime.Now;
            Make = dto.Make;
            Mileage = dto.Mileage;
            Model = dto.Model;
            Id = id;
        }

        public DateTime CreatedAt { get; private set; }

        [Required(AllowEmptyStrings = false)]
        public string Make { get; set; }

        public int Mileage { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Model { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Id { get; set; } // VIN - Entity Frameworks requires this to be called Id.

        public override string ToString()
        {
            return $"Make: {Make} - mileage: {Mileage} - Model: {Model} - Vin: {Id} - Created at: {CreatedAt}";
        }
    }
}