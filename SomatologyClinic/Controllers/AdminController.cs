using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SomatologyClinic.Data;
using System.Linq;
using System.Threading.Tasks;
using SomatologyClinic.Models.ViewModels;


namespace SomatologyClinic.Controllers
{
    [Authorize(Roles = "Admin")]


    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();

            var viewModel = new UserDetailsViewModel
            {
                User = user,
                Roles = userRoles,
                AllRoles = allRoles
            };

            return View(viewModel);
        }


        public IActionResult Create()
        {
            var roles = _roleManager.Roles.ToList();
            var viewModel = new CreateUserViewModel { AllRoles = roles };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (model.SelectedRoles != null)
                    {
                        await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                    }
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            model.AllRoles = _roleManager.Roles.ToList();
            return View(model);
        }



        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();

            var viewModel = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = userRoles,
                AllRoles = allRoles
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditUserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Email = model.Email;
                user.UserName = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var rolesToRemove = userRoles.Except(model.SelectedRoles ?? new List<string>());
                    var rolesToAdd = model.SelectedRoles?.Except(userRoles) ?? new List<string>();

                    await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    await _userManager.AddToRolesAsync(user, rolesToAdd);

                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            model.AllRoles = _roleManager.Roles.ToList();
            return View(model);
        }


        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Bookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Treatment)
                .OrderByDescending(b => b.BookingDateTime)
                .ToListAsync();

            return View(bookings);
        }

        public async Task<IActionResult> StaffActions()
        {
            var staffActions = await _context.Bookings
                .Where(b => b.LastUpdatedBy != null)
                .OrderByDescending(b => b.LastUpdatedAt)
                .Select(b => new StaffActionViewModel
                {
                    BookingId = b.Id,
                    TreatmentName = b.Treatment.Name,
                    CustomerName = b.User.UserName,
                    StaffName = b.LastUpdatedBy,
                    Action = b.Status.ToString(),
                    ActionDate = b.LastUpdatedAt
                })
                .ToListAsync();

            return View(staffActions);
        }







    }
}
