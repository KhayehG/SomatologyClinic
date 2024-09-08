namespace SomatologyClinic.Models.ViewModels
{
    public class StudentDashboardViewModel
    {
        public Student Student { get; set; }
        public List<Booking> UpcomingBookings { get; set; }
        public int CompletedTreatments { get; set; }
    }
}
