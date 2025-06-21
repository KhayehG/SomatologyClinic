using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
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
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ForgotPasswordModel> _logger;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender, ILogger<ForgotPasswordModel> logger)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation($"Attempting to reset password for email: {Input.Email}");
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    _logger.LogWarning($"Password reset attempted for non-existent or unconfirmed user: {Input.Email}");
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                _logger.LogInformation($"Generated password reset URL: {callbackUrl}");

                try
                {
                    _logger.LogInformation($"Attempting to send password reset email to {Input.Email}");
                    await _emailSender.SendEmailAsync(
                        Input.Email,
                        "Reset Your Password",
                        $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    _logger.LogInformation($"Password reset email sent successfully to {Input.Email}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error sending password reset email to {Input.Email}: {ex.Message}");
                    _logger.LogError($"Stack trace: {ex.StackTrace}");
                    ModelState.AddModelError(string.Empty, "Error sending password reset email. Please try again later.");
                    return Page();
                }

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }


    }
}
