using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SomatologyClinic.Data;
using SomatologyClinic.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SomatologyClinic.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Booking
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var bookings = await _context.Bookings
                .Include(b => b.Treatment)
                .Include(b => b.User)
                .Where(b => b.UserId == user.Id)
                .ToListAsync();
            return View(bookings);
        }

        // GET: Booking/Create
        public IActionResult Create()
        {
            ViewData["TreatmentId"] = new SelectList(_context.Treatments, "Id", "Name");
            return View();
        }

        // POST: Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TreatmentId,BookingDateTime")] Booking booking)
        {
            if (!ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                booking.UserId = user.Id;
                booking.Status = BookingStatus.Pending;
                booking.LastUpdatedBy = user.UserName; // Add this line
                booking.LastUpdatedAt = DateTime.UtcNow; // Add this line if it's not set by default
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TreatmentId"] = new SelectList(_context.Treatments, "Id", "Name", booking.TreatmentId);
            return View(booking);
        }


        // GET: Booking/ManageBookings (for staff)
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> ManageBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Treatment)
                .Include(b => b.User)
                .Where(b => b.Status == BookingStatus.Pending)
                .ToListAsync();
            return View(bookings);
        }

        // POST: Booking/ApproveReject
        [HttpPost]
        [Authorize(Roles = "Staff")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveReject(int id, bool approve, string staffNotes)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            booking.Status = approve ? BookingStatus.Approved : BookingStatus.Rejected;
            booking.StaffNotes = staffNotes;
            booking.LastUpdatedBy = User.Identity.Name;
            booking.LastUpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageBookings));
        }



        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> History()
        {
            var user = await _userManager.GetUserAsync(User);
            var bookings = await _context.Bookings
                .Include(b => b.Treatment)
                .Where(b => b.UserId == user.Id && b.Status == BookingStatus.Approved)
                .OrderByDescending(b => b.BookingDateTime)
                .ToListAsync();
            return View(bookings);
        }




    }
}
