namespace SomatologyClinic.Models.ViewModels
{
    public class BookingViewModel
    {
        public int Id { get; set; }
        public string TreatmentName { get; set; }
        public DateTime BookingDateTime { get; set; }
        public BookingStatus Status { get; set; }
        public decimal Price { get; set; }
    }

}
