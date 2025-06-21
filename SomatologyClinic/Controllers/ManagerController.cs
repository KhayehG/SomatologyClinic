using Microsoft.AspNetCore.Mvc;
using SomatologyClinic.Models;
using Microsoft.AspNetCore.Identity;
using SomatologyClinic.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using SomatologyClinic.Models.ViewModels;

//[ApiController]
//[Route("api/[controller]")]
//public class ManagerController : Controller
//{
//    private readonly UserManager<ApplicationUser> _userManager;
//    private readonly RoleManager<IdentityRole> _roleManager;
//    private readonly ApplicationDbContext _context;
//    private readonly Manager _manager = new Manager();

//    public ManagerController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
//    {
//        _userManager = userManager;
//        _roleManager = roleManager;
//        _context = context;
//    }

//    [HttpPost("addTherapist")]
//    public IActionResult AddTherapist(Therapist therapist)
//    {
//        _manager.AddTherapist(therapist);
//        return Ok();
//    }

//    [HttpPost("assignBooking")]
//    public IActionResult AssignBooking(Booking booking, int therapistId)
//    {
//        try
//        {
//            _manager.AssignBooking(booking, therapistId);
//            return Ok();
//        }
//        catch (Exception ex)
//        {
//            return BadRequest(ex.Message);
//        }
//    }

//    [HttpGet("getTherapistsBySpecialty")]
//    public IActionResult GetTherapistsBySpecialty(string specialty)
//    {
//        var therapists = _manager.GetTherapistsBySpecialty(specialty);
//        return Ok(therapists);
//    }

//    [HttpPut("editTherapist")]
//    public IActionResult EditTherapist(int therapistId, string name, string specialty)
//    {
//        try
//        {
//            _manager.EditTherapist(therapistId, name, specialty);
//            return Ok();
//        }
//        catch (Exception ex)
//        {
//            return BadRequest(ex.Message);
//        }
//    }

//    [HttpDelete("deleteTherapist")]
//    public IActionResult DeleteTherapist(int therapistId)
//    {
//        try
//        {
//            _manager.DeleteTherapist(therapistId);
//            return Ok();
//        }
//        catch (Exception ex)
//        {
//            return BadRequest(ex.Message);
//        }
//    }
//}
[Authorize]
public class ManagerController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public ManagerController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        var bookings = await _context.Bookings.ToListAsync();
        return View(bookings);
    }
    [HttpGet]
   

    public async Task<IActionResult> SelectTherapist(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
        {
            return NotFound();
        }

        var therapists = await _context.Therapists
            .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name })
            .ToListAsync();

        var viewModel = new SelectTherapistViewModel
        {
            BookingId = booking.Id,
            Therapists = therapists
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AssignTherapist(SelectTherapistViewModel model)
    {
        var booking = await _context.Bookings.FindAsync(model.BookingId);
        if (booking == null)
        {
            return NotFound();
        }

        booking.TherapistId = model.TherapistId;
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
    //[HttpGet]
    //public async Task<IActionResult> Index()
    //{
    //    return View(await _context.Therapists.ToListAsync());
    //}
    //public IActionResult AddTherapist()
    //{
    //    return View();
    //}

    //[HttpPost]
    //public async Task<IActionResult> AddTherapist(Therapist therapist)
    //{
    //    _context.Therapists.Add(therapist);
    //    await _context.SaveChangesAsync();
    //    return RedirectToAction("Index");  // Adjust as necessary
    //}
    ////GET: ManagerController/AssignBooking
    //public IActionResult AssignBooking()
    //{
    //    return View();
    //}

    //[HttpPost]
    //public async Task<IActionResult> AssignBooking(Booking booking, int therapistId)
    //{
    //    try
    //    {
    //        var therapist = await _context.Therapists.FindAsync(therapistId);
    //        if (therapist != null)
    //        {
    //            booking.TherapistId = therapistId;
    //            _context.Bookings.Add(booking);
    //            await _context.SaveChangesAsync();
    //            return RedirectToAction("Index");  // Adjust as necessary
    //        }
    //        return NotFound("Therapist not found");
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }
    //}

    [HttpGet]
    public async Task<IActionResult> GetTherapistsBySpecialty(string specialty)
    {
        var therapists = await _context.Therapists.Where(t => t.Specialty == specialty).ToListAsync();
        return View(therapists);  // Adjust as necessary
    }

    [HttpPut]
    public async Task<IActionResult> EditTherapist(int therapistId, Therapist updatedTherapist)
    {
        try
        {
            var therapist = await _context.Therapists.FindAsync(therapistId);
            if (therapist != null)
            {
                therapist.Name = updatedTherapist.Name;
                therapist.Specialty = updatedTherapist.Specialty;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");  // Adjust as necessary
            }
            return NotFound("Therapist not found");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTherapist(int therapistId)
    {
            try
            {
                var therapist = await _context.Therapists.FindAsync(therapistId);
                if (therapist != null)
                {
                    _context.Therapists.Remove(therapist);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");  // Adjust as necessary
                }
                return NotFound("Therapist not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
    }




// GET: ManagerController
        //public ActionResult Index()
        //{
        //    return View();
        //}


        // GET: ManagerController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ManagerController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ManagerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ManagerController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
    }
    
        // POST: ManagerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ManagerController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ManagerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }

