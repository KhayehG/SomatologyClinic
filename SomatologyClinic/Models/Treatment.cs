using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SomatologyClinic.Models
{
    public class Treatment
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 minute.")]
        public int Duration { get; set; } // Duration in minutes

        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<Booking> Bookings { get; set; }

        public string Icon { get; set; }  

    }
}
