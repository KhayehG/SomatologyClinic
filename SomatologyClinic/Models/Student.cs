using SomatologyClinic.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SomatologyClinic.Models
{
    public class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Registration Date")]
        public DateTime RegistrationDate { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Required]
        [Display(Name = "Student Number")]
        public string StudentNumber { get; set; }
    }
}
