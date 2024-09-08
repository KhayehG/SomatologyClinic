using SomatologyClinic.Data;
using System.ComponentModel.DataAnnotations;

namespace SomatologyClinic.Models
{
    public class Booking
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
        public DateTime BookingDateTime { get; set; }

        public BookingStatus Status { get; set; }

        public string? StaffNotes { get; set; }

        public string? LastUpdatedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public decimal? Price { get; set; }  

    }

    public enum BookingStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
