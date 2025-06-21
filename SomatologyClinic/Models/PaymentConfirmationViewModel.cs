namespace SomatologyClinic.Models
{
    public class PaymentConfirmationViewModel
    {
        public int PaymentId { get; set; }
        public string TreatmentName { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string CardType { get; set; }
        public string LastFour { get; set; }
        public string TransactionId { get; set; }
    }
}
