using SomatologyClinic.Data;
using System;
using System.ComponentModel.DataAnnotations;

namespace SomatologyClinic.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        public int TreatmentId { get; set; }
        public Treatment Treatment { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDate { get; set; }

        public bool IsApproved { get; set; }

      
    }
}
