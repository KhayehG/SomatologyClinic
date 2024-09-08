using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SomatologyClinic.Data;
using SomatologyClinic.Models;
using SomatologyClinic.Models.ViewModels;
using System.Threading.Tasks;
using SomatologyClinic.Services;

namespace SomatologyClinic.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPaymentService _paymentService;

        public PaymentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IPaymentService paymentService)
        {
            _context = context;
            _userManager = userManager;
            _paymentService = paymentService;
        }

        public async Task<IActionResult> SelectPaymentMethod(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Treatment)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
            {
                return NotFound();
            }

            var viewModel = new PaymentViewModel
            {
                BookingId = booking.Id,
                TreatmentName = booking.Treatment.Name,
                Amount = booking.Treatment.Price,
                BookingDateTime = booking.BookingDateTime
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(PaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("SelectPaymentMethod", model);
            }

            var booking = await _context.Bookings
                .Include(b => b.Treatment)
                .FirstOrDefaultAsync(b => b.Id == model.BookingId);

            if (booking == null)
            {
                return NotFound();
            }

            var paymentResult = await _paymentService.ProcessPayment(
                model.SelectedPaymentMethod,
                booking.Treatment.Price,
                "ZAR",
                booking.Id.ToString(),
                model.PaymentToken,
                model.Installments);

            if (paymentResult.Success)
            {
                var user = await _userManager.GetUserAsync(User);
                var payment = new Payment
                {
                    UserId = user.Id,
                    BookingId = booking.Id,
                    Amount = booking.Treatment.Price,
                    PaymentDate = DateTime.UtcNow,
                    PaymentMethod = model.SelectedPaymentMethod,
                    TransactionId = paymentResult.TransactionId,
                    Status = "Completed"
                };

                _context.Payments.Add(payment);
                booking.Status = BookingStatus.Approved;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Payment successful. Your booking has been confirmed.";
                return RedirectToAction("MyBookings", "Student");
            }
            else
            {
                ModelState.AddModelError("", paymentResult.Message);
                return View("SelectPaymentMethod", model);
            }
        }

        public async Task<IActionResult> PaymentConfirmation(int paymentId)
        {
            var payment = await _context.Payments
                .Include(p => p.Booking)
                .ThenInclude(b => b.Treatment)
                .FirstOrDefaultAsync(p => p.Id == paymentId);

            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }
    }
}
