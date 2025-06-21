using System.ComponentModel.DataAnnotations;

namespace SomatologyClinic.Models
{
    public class SpecialPackage
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; } // Package Price

        [Required]
        public List<Treatment> Treatments { get; set; } = new List<Treatment>(); // List of treatments in the package

        public int Duration
        {
            get
            {
                // Total duration as the sum of all treatment durations
                return Treatments.Sum(t => t.Duration);
            }
        }
    }
}
