using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using SomatologyClinic.Models;

namespace SomatologyClinic.Data
{
    public class DbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public DbInitializer(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task SeedRolesAsync()
        {
            string[] roleNames = { "Admin", "Student", "Staff", "Customer","Manager" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        public async Task SeedAdminUserAsync()
        {
            var adminEmail = "admin@dut.ac.za";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Mickey",
                    LastName = "Mouse",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, "Password1!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }

            var studentEmail = "21942332@dut4life.ac.za";
            var studentUser = await _userManager.FindByEmailAsync(studentEmail);
            if (studentUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = studentEmail,
                    Email = studentEmail,
                    FirstName = "SupervisorFirstName",
                    LastName = "SupervisorLastName",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, "Password123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Student");
                }
            }
        }

        public async Task SeedTreatmentsAsync()
        {
            if (!await _context.Treatments.AnyAsync())
            {
                var treatments = new List<Treatment>
        {
            new Treatment { Name = "Swedish Massage", Price = 105.00m, Duration = 60, Icon = "spa" },
            new Treatment { Name = "Back & Neck Massage", Price = 100.00m, Duration = 30, Icon = "accessibility" },
            new Treatment { Name = "Hot Stone Full Body Massage", Price = 180.00m, Duration = 60, Icon = "hot_tub" },
            new Treatment { Name = "Full Body Exfoliation", Price = 150.00m, Duration = 30, Icon = "waves" },
            new Treatment { Name = "Reflexology", Price = 75.00m, Duration = 45, Icon = "foot_care" },
            new Treatment { Name = "Slimming Treatment", Price = 65.00m, Duration = 45, Icon = "fitness_center" },
            new Treatment { Name = "G5 Massage", Price = 65.00m, Duration = 30, Icon = "vibration" },
            new Treatment { Name = "Sauna Session", Price = 50.00m, Duration = 30, Icon = "whatshot" },
            new Treatment { Name = "Steam Bath", Price = 50.00m, Duration = 30, Icon = "cloud" },
            new Treatment { Name = "Jacuzzi Session", Price = 50.00m, Duration = 30, Icon = "pool" },
            new Treatment { Name = "Body Statistical Analysis", Price = 50.00m, Duration = 30, Icon = "assessment" },
            new Treatment { Name = "Eyebrow Tinting", Price = 25.00m, Duration = 15, Icon = "brush" },
            new Treatment { Name = "Eyelash Tinting", Price = 15.00m, Duration = 15, Icon = "visibility" },
            new Treatment { Name = "Eyebrow Waxing", Price = 20.00m, Duration = 15, Icon = "content_cut" },
            new Treatment { Name = "Upperlip Waxing", Price = 20.00m, Duration = 10, Icon = "face" },
            new Treatment { Name = "Chin Waxing", Price = 20.00m, Duration = 10, Icon = "face_retouching_natural" },
            new Treatment { Name = "Full Face Waxing", Price = 65.00m, Duration = 15, Icon = "face_unlock" },
            new Treatment { Name = "Underarm Waxing", Price = 45.00m, Duration = 15, Icon = "psychology_alt" },
            new Treatment { Name = "Bikini Waxing", Price = 65.00m, Duration = 20, Icon = "favorite_border" },
            new Treatment { Name = "Brazilian Waxing", Price = 80.00m, Duration = 30, Icon = "favorite" },
            new Treatment { Name = "Full Leg Waxing", Price = 130.00m, Duration = 30, Icon = "airline_seat_legroom_extra" },
            new Treatment { Name = "Half Leg Waxing", Price = 80.00m, Duration = 20, Icon = "airline_seat_legroom_normal" },
            new Treatment { Name = "Chest Waxing", Price = 140.00m, Duration = 30, Icon = "view_agenda" },
            new Treatment { Name = "Basic Manicure", Price = 120.00m, Duration = 45, Icon = "back_hand" },
            new Treatment { Name = "Basic Pedicure", Price = 140.00m, Duration = 60, Icon = "footprint" },
            new Treatment { Name = "Pedi Heel Peel", Price = 200.00m, Duration = 75, Icon = "healing" },
            new Treatment { Name = "Nail Paint Only", Price = 20.00m, Duration = 15, Icon = "palette" },
            new Treatment { Name = "Deep Cleanse Facial", Price = 150.00m, Duration = 60, Icon = "face_retouching_natural" },
            new Treatment { Name = "Environ Facial", Price = 180.00m, Duration = 60, Icon = "eco" },
            new Treatment { Name = "Dermologica Facial", Price = 180.00m, Duration = 60, Icon = "face_3" },
            new Treatment { Name = "Specialised Facial", Price = 200.00m, Duration = 90, Icon = "star" },
            new Treatment { Name = "Spider Vein Treatment", Price = 125.00m, Duration = 5, Icon = "pest_control" },
            new Treatment { Name = "Pigmentation Treatment", Price = 170.00m, Duration = 10, Icon = "texture" },
            new Treatment { Name = "Skin Tag Removal", Price = 255.00m, Duration = 15, Icon = "cut" },
            new Treatment { Name = "Cool Peels", Price = 270.00m, Duration = 60, Icon = "ac_unit" },
            new Treatment { Name = "Microneedling", Price = 215.00m, Duration = 60, Icon = "grid_3x3" }
        };

                await _context.Treatments.AddRangeAsync(treatments);
                await _context.SaveChangesAsync();
            }
        }



        public async Task SeedStaffMemberAsync()
        {
            var staffEmail = "staff@dut.ac.za";
            var staffUser = await _userManager.FindByEmailAsync(staffEmail);
            if (staffUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = staffEmail,
                    Email = staffEmail,
                    FirstName = "John",
                    LastName = "Doe",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, "StaffPassword123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Staff");

                    var staff = new Staff
                    {
                        ApplicationUserId = user.Id,
                        StaffId = "STAFF001",
                        Department = "Somatology"
                    };

                    await _context.Staffs.AddAsync(staff);
                    await _context.SaveChangesAsync();
                }
            }
        }
        public async Task SeedManagerMemberAsync()
        {
            var managerEmail = "Manager01@dut.ac.za";
            var managerUser = await _userManager.FindByEmailAsync(managerEmail);
            if(managerUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = managerEmail,
                    Email = managerEmail,
                    FirstName = "Manager",
                    LastName = "Manager",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, "ManagerPassword123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Manager");

                }
            }

        }
    }
}
