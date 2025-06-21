using SomatologyClinic.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SomatologyClinic.Models
{
    public class Staff
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        [Display(Name = "Staff Number")]
        public string StaffId { get; set; }

        [Required]
        [Display(Name = "Department")]
        public string Department { get; set; }
    }
}
