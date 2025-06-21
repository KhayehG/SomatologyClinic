namespace SomatologyClinic.Models.ViewModels
{
    public class MultipleBookingViewModel
    {
        public List<int> SelectedTreatmentIds { get; set; } = new List<int>(); // Multiple Treatments
        public DateTime BookingDateTime { get; set; } // Booking Date and Time
    }
}

