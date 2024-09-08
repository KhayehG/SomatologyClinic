namespace SomatologyClinic.Models.ViewModels
{
    public class StaffDashboardViewModel
    {
        public int PendingBookingsCount { get; set; }
        public int TodayBookingsCount { get; set; }
        public int TotalCustomersCount { get; set; }
        public int TotalTreatmentsCount { get; set; }
    }

}
