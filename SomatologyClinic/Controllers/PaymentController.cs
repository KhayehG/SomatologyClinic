using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SomatologyClinic.Data;
using SomatologyClinic.Models;
using SomatologyClinic.Models.ViewModels;
using SomatologyClinic.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SomatologyClinic.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<PaymentController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> SelectPaymentMethod(int bookingId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Treatment)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == user.Id);

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(PaymentViewModel model)
        {
            _logger.LogInformation($"Payment for BookingId: {model.BookingId}");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User not found");
                return Json(new { success = false, message = "User not found" });
            }

            var booking = await _context.Bookings
                .Include(b => b.Treatment)
                .FirstOrDefaultAsync(b => b.Id == model.BookingId && b.UserId == user.Id);

            if (booking == null)
            {
                _logger.LogWarning($"Booking not found for BookingId: {model.BookingId} and UserId: {user.Id}");
                return Json(new { success = false, message = "Booking not found" });
            }

            try
            {
                var payment = new Payment
                {
                    UserId = user.Id,
                    BookingId = booking.Id,
                    Amount = booking.Treatment.Price,
                    PaymentDate = DateTime.UtcNow,
                    PaymentMethod = model.SelectedPaymentMethod,
                    TransactionId = Guid.NewGuid().ToString(),
                    Status = "Completed",
                    CardType = model.CardType ?? "N/A",
                    LastFour = model.LastFour ?? "N/A"
                };

                _context.Payments.Add(payment);
                booking.Status = BookingStatus.Approved;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Payment successful for BookingId: {booking.Id}, PaymentId: {payment.Id}");

                return RedirectToAction("PaymentConfirmation", new { paymentId = payment.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing payment for BookingId: {model.BookingId}");
                return View("Error");
            }
        }


        public async Task<IActionResult> PaymentConfirmation(int paymentId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Booking)
                .ThenInclude(b => b.Treatment)
                .FirstOrDefaultAsync(p => p.Id == paymentId && p.UserId == user.Id);

            if (payment == null)
            {
                _logger.LogWarning($"Payment not found for PaymentId: {paymentId} and UserId: {user.Id}");
                return NotFound();
            }

            var viewModel = new PaymentConfirmationViewModel
            {
                PaymentId = payment.Id,
                TreatmentName = payment.Booking.Treatment.Name,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,
                CardType = payment.CardType,
                LastFour = payment.LastFour,
                TransactionId = payment.TransactionId
            };

            return View(viewModel);
        }
    }
}