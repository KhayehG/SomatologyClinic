using SomatologyClinic.Data;
using SomatologyClinic.Models;

public class Payment
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public int BookingId { get; set; }
    public Booking Booking { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; }
    public string TransactionId { get; set; }
    public string Status { get; set; }
}
