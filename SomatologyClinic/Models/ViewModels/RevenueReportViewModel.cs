namespace SomatologyClinic.Models.ViewModels
{
    public class RevenueReportViewModel
    {
        public decimal TotalRevenue { get; set; }
        public List<RevenueByTreatmentViewModel> RevenueByTreatment { get; set; }
        public List<RevenueByMonthViewModel> RevenueByMonth { get; set; }
    }
}
