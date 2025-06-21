namespace SomatologyClinic.Models.ViewModels
{
    public class PaymentViewModel
    {
        public int BookingId { get; set; }
        public string TreatmentName { get; set; }
        public decimal Amount { get; set; }
        public DateTime BookingDateTime { get; set; }
        public string SelectedPaymentMethod { get; set; }
        public string PaymentToken { get; set; }
        public int? Installments { get; set; }
        public string CardType { get; set; }
        public string LastFour { get; set; }
    }



}
