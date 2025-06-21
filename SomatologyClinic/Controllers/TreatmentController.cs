using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SomatologyClinic.Data;
using SomatologyClinic.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SomatologyClinic.Models.ViewModels;

namespace SomatologyClinic.Controllers
{
    [Authorize(Roles = "Admin,Staff")] // Only Admin and Staff can access treatment management
    public class TreatmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public TreatmentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //// GET: Treatment
        public async Task<IActionResult> Index()
        {
            return View(await _context.Treatments.ToListAsync());
        }



        // GET: Treatment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatment = await _context.Treatments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (treatment == null)
            {
                return NotFound();
            }

            return View(treatment);
        }
        



        // GET: Treatment/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Treatment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price,Duration")] Treatment treatment)
        {
            if (!ModelState.IsValid)
            {
                treatment.Icon = string.IsNullOrEmpty(treatment.Icon) ? "spa" : treatment.Icon;
                _context.Add(treatment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(treatment);
        }

        // GET: Treatment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatment = await _context.Treatments.FindAsync(id);
            if (treatment == null)
            {
                return NotFound();
            }
            return View(treatment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Duration")] Treatment treatment)
        {
            if (id != treatment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(treatment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TreatmentExists(treatment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(treatment);
        }


        // GET: Treatment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatment = await _context.Treatments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (treatment == null)
            {
                return NotFound();
            }

            return View(treatment);
        }

        // POST: Treatment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var treatment = await _context.Treatments.FindAsync(id);
            _context.Treatments.Remove(treatment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TreatmentExists(int id)
        {
            return _context.Treatments.Any(e => e.Id == id);
        }
    }
}


