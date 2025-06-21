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
        public int? TreatmentId { get; set; }
        public Treatment Treatment { get; set; }
        public int? TherapistId { get; set; }
        public Therapist Therapist { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime BookingDateTime { get; set; }

        public BookingStatus Status { get; set; }

        public string? StaffNotes { get; set; }

        public string? LastUpdatedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public decimal? Price { get; set; }
        public int? SpecialPackageId { get; set; } // If booking is for a special package
        public SpecialPackage SpecialPackage { get; set; }


    }
    //public class Booking
    //{
    //    public int Id { get; set; }
    //    public string UserId { get; set; }
    //    public DateTime BookingDateTime { get; set; }
    //    public BookingStatus Status { get; set; }
    //    public string StaffNotes { get; set; }
    //    public string LastUpdatedBy { get; set; }
    //    public DateTime LastUpdatedAt { get; set; }

    //    public ApplicationUser User { get; set; }
    ////    public ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();
    ////}




    public enum BookingStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
