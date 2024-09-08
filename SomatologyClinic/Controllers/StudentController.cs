using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SomatologyClinic.Data;
using SomatologyClinic.Models;
using SomatologyClinic.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SomatologyClinic.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }



        public async Task<IActionResult> SelectTreatment()
        {
            var treatments = await _context.Treatments.ToListAsync();
            return View(treatments);
        }

        [HttpPost]
        public async Task<IActionResult> SelectTreatment(int treatmentId)
        {
            var treatment = await _context.Treatments.FindAsync(treatmentId);
            if (treatment == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var booking = new Booking
            {
                UserId = user.Id,
                TreatmentId = treatmentId,
                BookingDateTime = DateTime.Now,
                Status = BookingStatus.Pending,
                LastUpdatedBy = user.Id, // Set the LastUpdatedBy field
                LastUpdatedAt = DateTime.UtcNow // Set the LastUpdatedAt field
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction("SelectPaymentMethod", "Payment", new { bookingId = booking.Id });
        }



        // GET: Student/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students
                .Include(s => s.ApplicationUser)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (student == null)
            {
                return NotFound();
            }

            var viewModel = new StudentDashboardViewModel
            {
                Student = student,
                UpcomingBookings = await _context.Bookings
                    .Include(b => b.Treatment)
                    .Where(b => b.UserId == user.Id && b.BookingDateTime > DateTime.Now)
                    .OrderBy(b => b.BookingDateTime)
                    .Take(5)
                    .ToListAsync(),
                CompletedTreatments = await _context.Bookings
                    .Include(b => b.Treatment)
                    .Where(b => b.UserId == user.Id && b.BookingDateTime <= DateTime.Now && b.Status == BookingStatus.Approved)
                    .CountAsync()
            };

            return View(viewModel);
        }

        // GET: Student/BookTreatment

        public IActionResult BookTreatment()
        {
            var treatments = _context.Treatments.Select(t => new
            {
                id = t.Id,
                name = t.Name,
                price = t.Price,
                icon = string.IsNullOrEmpty(t.Icon) ? "spa" : t.Icon  // Default icon if not set
            }).ToList();

            ViewBag.Treatments = treatments;
            return View();
        }


        // POST: Student/BookTreatment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookTreatment(int TreatmentId, DateTime BookingDateTime)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var treatment = await _context.Treatments.FindAsync(TreatmentId);
                if (treatment == null)
                {
                    return NotFound();
                }

                var booking = new Booking
                {
                    UserId = user.Id,
                    TreatmentId = TreatmentId,
                    BookingDateTime = BookingDateTime,
                    Status = BookingStatus.Pending,
                    LastUpdatedBy = user.UserName,
                    LastUpdatedAt = DateTime.UtcNow,
                    Price = treatment.Price  // Add this line to store the price at the time of booking
                };
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Dashboard));
            }
            // If we got this far, something failed; redisplay form
            var treatments = _context.Treatments.Select(t => new
            {
                id = t.Id,
                name = t.Name,
                price = t.Price,
                icon = t.Icon  // Now using the Icon from the database
            }).ToList();
            ViewBag.Treatments = treatments;
            return View();
        }



        // GET: Student/MyBookings
        public async Task<IActionResult> MyBookings()
        {
            var user = await _userManager.GetUserAsync(User);
            var bookings = await _context.Bookings
                .Include(b => b.Treatment)
                .Where(b => b.UserId == user.Id)
                .OrderByDescending(b => b.BookingDateTime)
                .ToListAsync();

            return View(bookings);
        }


        // GET: Student/TreatmentHistory
        public async Task<IActionResult> TreatmentHistory()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    // Log this unexpected scenario
                    return RedirectToAction("Login", "Account");
                }

                var bookings = await _context.Bookings
                    .Include(b => b.Treatment)
                    .Where(b => b.UserId == user.Id &&
                                b.Status == BookingStatus.Approved &&
                                b.BookingDateTime <= DateTime.Now)
                    .OrderByDescending(b => b.BookingDateTime)
                    .ToListAsync();

                return View(bookings);
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }


        // POST: Student/CancelBooking/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null || booking.UserId != user.Id)
            {
                return NotFound();
            }

            if (booking.Status != BookingStatus.Pending || booking.BookingDateTime <= DateTime.Now)
            {
                return BadRequest("This booking cannot be cancelled.");
            }

            booking.Status = BookingStatus.Rejected;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyBookings));
        }


        // GET: Student/Profile
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.ApplicationUser)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (student == null)
            {
                return NotFound();
            }

            var viewModel = new StudentProfileViewModel
            {
                Id = student.Id,
                StudentNumber = student.StudentNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = student.DateOfBirth,
                Gender = student.Gender,
                RegistrationDate = student.RegistrationDate,
                IsActive = student.IsActive
            };

            return View(viewModel);
        }

        // POST: Student/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(StudentProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(model.Id);
            if (student == null)
            {
                return NotFound();
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;

            student.DateOfBirth = model.DateOfBirth;
            student.Gender = model.Gender;
            // Note: You might want to control who can change IsActive status
            // student.IsActive = model.IsActive;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = "Your profile has been updated";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }



    }
}
