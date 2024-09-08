namespace SomatologyClinic.Models.ViewModels
{
    public class BookingStatisticsViewModel
    {
        public int TotalBookings { get; set; }
        public List<PopularTreatmentViewModel> PopularTreatments { get; set; }
        public List<BookingsByDayViewModel> BookingsByDayOfWeek { get; set; }
    }
}
