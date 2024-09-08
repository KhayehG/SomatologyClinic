namespace SomatologyClinic.Models.ViewModels
{
    public class StaffActionViewModel
    {
        public int BookingId { get; set; }
        public string TreatmentName { get; set; }
        public string CustomerName { get; set; }
        public string StaffName { get; set; }
        public string Action { get; set; }
        public DateTime ActionDate { get; set; }
    }
}
