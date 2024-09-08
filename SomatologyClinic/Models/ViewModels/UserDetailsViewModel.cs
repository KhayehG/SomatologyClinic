using Microsoft.AspNetCore.Identity;
using SomatologyClinic.Data;

namespace SomatologyClinic.Models.ViewModels
{
    public class UserDetailsViewModel
    {
        public ApplicationUser User { get; set; }
        public IList<string> Roles { get; set; }
        public IList<IdentityRole> AllRoles { get; set; }
    }
}
