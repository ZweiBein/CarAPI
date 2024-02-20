using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CarAPI.Models
{
    public class Car
    {
        public Car(string make, int mileage, string model, string vin)
        {
            CreatedAt = DateTime.Now;
            Make = make;
            Mileage = mileage;
            Model = model;
            Vin = vin;
        }

        public DateTime CreatedAt { get; private set; }

        [Required(AllowEmptyStrings = false)]
        public string Make { get; set; }

        public int Mileage { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Model { get; set; }

        [Key]
        [Required(AllowEmptyStrings = false)]
        public string Vin { get; set; }

        public override string ToString()
        {
            return $"Make: {Make} - mileage: {Mileage} - Model: {Model} - VIN: {Vin} - Created at: {CreatedAt}";
        }
    }
}