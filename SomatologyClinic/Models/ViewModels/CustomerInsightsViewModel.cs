namespace SomatologyClinic.Models.ViewModels
{
    public class CustomerInsightsViewModel
    {
        public int TotalCustomers { get; set; }
        public List<TopCustomerViewModel> TopCustomers { get; set; }
        public double CustomerRetentionRate { get; set; }
    }
}
