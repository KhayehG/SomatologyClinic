using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SomatologyClinic.Data;
using SomatologyClinic.Models;
using System.Linq;
using System.Threading.Tasks;
using SomatologyClinic.Models.ViewModels;



namespace SomatologyClinic.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StaffController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Staff/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var viewModel = new StaffDashboardViewModel
            {
                PendingBookingsCount = await _context.Bookings.CountAsync(b => b.Status == BookingStatus.Pending),
                TodayBookingsCount = await _context.Bookings.CountAsync(b => b.BookingDateTime.Date == DateTime.Today && b.Status == BookingStatus.Approved),
                TotalCustomersCount = await _context.Customers.CountAsync(),
                TotalTreatmentsCount = await _context.Treatments.CountAsync()
            };

            return View(viewModel);
        }

        // GET: Staff/BookingCalendar
        public async Task<IActionResult> BookingCalendar()
        {
            var startDate = DateTime.Today.AddDays(-DateTime.Today.Day + 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var bookings = await _context.Bookings
                .Include(b => b.Treatment)
                .Include(b => b.User)
                .Where(b => b.BookingDateTime >= startDate && b.BookingDateTime <= endDate)
                .ToListAsync();

            var viewModel = bookings.Select(b => new BookingCalendarViewModel
            {
                Id = b.Id,
                Title = $"{b.Treatment.Name} - {b.User.FirstName} {b.User.LastName}",
                Start = b.BookingDateTime,
                End = b.BookingDateTime.AddMinutes(b.Treatment.Duration),
                Color = GetBookingColor(b.Status)
            }).ToList();

            return View(viewModel);
        }

        private static string GetBookingColor(BookingStatus status)
        {
            return status switch
            {
                BookingStatus.Pending => "#FFA500",   // Orange
                BookingStatus.Approved => "#32CD32",  // Lime Green
                BookingStatus.Rejected => "#FF6347", // Tomato
                _ => "#808080"                        // Gray
            };
        }


        // GET: Staff/TreatmentManagement
        public async Task<IActionResult> TreatmentManagement()
        {
            var treatments = await _context.Treatments.ToListAsync();
            return View(treatments);
        }

        // GET: Staff/CustomerManagement
        public async Task<IActionResult> CustomerManagement()
        {
            var customers = await _context.Customers
                .Include(c => c.ApplicationUser)
                .ToListAsync();
            return View(customers);
        }

        // GET: Staff/Reports
        public IActionResult Reports()
        {
            return View();
        }




        /// add-ons 
        /// 
        // GET: Staff/BookingStatistics
        public async Task<IActionResult> BookingStatistics()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Treatment)
                .Where(b => b.Status == BookingStatus.Approved)
                .ToListAsync();

            var viewModel = new BookingStatisticsViewModel
            {
                TotalBookings = bookings.Count,
                PopularTreatments = bookings.GroupBy(b => b.Treatment.Name)
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .Select(g => new PopularTreatmentViewModel { Name = g.Key, Count = g.Count() })
                    .ToList(),
                BookingsByDayOfWeek = bookings.GroupBy(b => b.BookingDateTime.DayOfWeek)
                    .OrderBy(g => g.Key)
                    .Select(g => new BookingsByDayViewModel { DayOfWeek = g.Key, Count = g.Count() })
                    .ToList()
            };

            return View(viewModel);
        }

        // GET: Staff/RevenueReport
        public async Task<IActionResult> RevenueReport()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Treatment)
                .Where(b => b.Status == BookingStatus.Approved)
                .ToListAsync();

            var viewModel = new RevenueReportViewModel
            {
                TotalRevenue = bookings.Sum(b => b.Treatment.Price),
                RevenueByTreatment = bookings.GroupBy(b => b.Treatment.Name)
                    .Select(g => new RevenueByTreatmentViewModel { TreatmentName = g.Key, Revenue = g.Sum(b => b.Treatment.Price) })
                    .OrderByDescending(r => r.Revenue)
                    .ToList(),
                RevenueByMonth = bookings.GroupBy(b => new { b.BookingDateTime.Year, b.BookingDateTime.Month })
                    .Select(g => new RevenueByMonthViewModel
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Revenue = g.Sum(b => b.Treatment.Price)
                    })
                    .OrderByDescending(r => r.Year).ThenByDescending(r => r.Month)
                    .Take(12)
                    .ToList()
            };

            return View(viewModel);
        }

        

    
        // GET: Staff/GetBookings
        public async Task<IActionResult> GetBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Treatment)
                .Include(b => b.User)
                .Where(b => b.Status == BookingStatus.Approved)
                .Select(b => new
                {
                    id = b.Id,
                    title = $"{b.Treatment.Name} - {b.User.FirstName} {b.User.LastName}",
                    start = b.BookingDateTime,
                    end = b.BookingDateTime.AddHours(1) // Assuming each treatment takes 1 hour
                })
                .ToListAsync();

            return Json(bookings);
        }
    }
}
