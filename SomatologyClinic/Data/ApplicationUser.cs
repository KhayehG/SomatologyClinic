using Microsoft.AspNetCore.Identity;
using SomatologyClinic.Models;
using System.ComponentModel.DataAnnotations;

namespace SomatologyClinic.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

  
        // Navigation properties
        public virtual ICollection<Appointment > Appointments { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }

        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<Staff> Staffs { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }

        public ICollection<Booking> Bookings { get; set; }

    }
}