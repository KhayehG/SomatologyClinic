using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SomatologyClinic.Data;
using SomatologyClinic.Models;

namespace SomatologyClinic.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            // Student-specific fields
            [DataType(DataType.Date)]
            [Display(Name = "Date of Birth")]
            public DateTime? DateOfBirth { get; set; }

            public string Gender { get; set; }

            [Display(Name = "Student Number")]
            public string StudentNumber { get; set; }

            // Staff-specific fields
            [Display(Name = "Staff Number")]
            public string StaffId { get; set; }

            [Display(Name = "Department")]
            public string Department { get; set; }

            // Customer-specific fields
            [Phone]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Address")]
            public string Address { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            try
            {
                returnUrl ??= Url.Content("~/");
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

                string userType = DetermineUserType(Input.Email);
                _logger.LogInformation($"Attempting to register user of type {userType}");

                // Remove validation errors for fields that are not relevant to the current user type
                foreach (var key in ModelState.Keys.ToList())
                {
                    if ((userType != "Student" && key.StartsWith("Input.Student")) ||
                        (userType != "Staff" && key.StartsWith("Input.Staff")) ||
                        (userType != "Customer" && key.StartsWith("Input.Customer")))
                    {
                        ModelState.Remove(key);
                    }
                }

                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser
                    {
                        UserName = Input.Email,
                        Email = Input.Email,
                        FirstName = Input.FirstName,
                        LastName = Input.LastName
                    };

                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"User created a new account with password. User ID: {user.Id}");

                        switch (userType)
                        {
                            case "Student":
                                var student = new Student
                                {
                                    ApplicationUserId = user.Id,
                                    DateOfBirth = Input.DateOfBirth ?? DateTime.Now,
                                    Gender = Input.Gender ?? "Not Specified",
                                    StudentNumber = Input.StudentNumber ?? "Not Assigned",
                                    RegistrationDate = DateTime.Now,
                                    IsActive = true
                                };
                                await _userManager.AddToRoleAsync(user, "Student");
                                _context.Students.Add(student);
                                break;
                            case "Staff":
                                var staff = new Staff
                                {
                                    ApplicationUserId = user.Id,
                                    StaffId = Input.StaffId ?? "Not Assigned",
                                    Department = Input.Department ?? "Not Assigned"
                                };
                                await _userManager.AddToRoleAsync(user, "Staff");
                                _context.Staffs.Add(staff);
                                break;
                            default: // Customer
                                var customer = new Customer
                                {
                                    ApplicationUserId = user.Id,
                                    PhoneNumber = Input.PhoneNumber ?? "Not Provided",
                                    Address = Input.Address ?? "Not Provided"
                                };
                                await _userManager.AddToRoleAsync(user, "Customer");
                                _context.Customers.Add(customer);
                                break;
                        }

                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"Successfully created {userType} entity for user with ID {user.Id}");

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                            _logger.LogError($"User creation error: {error.Description}");
                        }
                    }
                }
                else
                {
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            _logger.LogError($"Model validation error: {error.ErrorMessage}");
                        }
                    }
                }

                // If we got this far, something failed, redisplay form
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception during registration: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
                return Page();
            }
        }

        private string DetermineUserType(string email)
        {
            if (email.EndsWith("@dut4life.ac.za", StringComparison.OrdinalIgnoreCase))
            {
                return "Student";
            }
            else if (email.EndsWith("@dut.ac.za", StringComparison.OrdinalIgnoreCase))
            {
                return "Staff";
            }
            else
            {
                return "Customer";
            }
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
