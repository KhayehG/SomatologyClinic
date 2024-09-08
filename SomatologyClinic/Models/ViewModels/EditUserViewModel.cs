using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SomatologyClinic.Models.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public IList<string> Roles { get; set; }
        public IList<string> SelectedRoles { get; set; }
        public IList<IdentityRole> AllRoles { get; set; }
    }
}
