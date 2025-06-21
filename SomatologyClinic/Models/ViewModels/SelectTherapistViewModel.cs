using Microsoft.AspNetCore.Mvc.Rendering;

namespace SomatologyClinic.Models.ViewModels
{
    public class SelectTherapistViewModel
    {
        public int BookingId { get; set; }
        public int TherapistId { get; set; }
        public List<SelectListItem> Therapists { get; set; }
    }

}
