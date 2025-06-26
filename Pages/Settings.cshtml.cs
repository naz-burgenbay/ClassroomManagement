using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassroomManagement.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

[Authorize]
public class SettingsModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public SettingsModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string StudId { get; set; }
    public string Specialization { get; set; }

    [BindProperty]
    public ChangeEmailModel ChangeEmail { get; set; }

    [BindProperty]
    public ChangePasswordModel ChangePassword { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    public class ChangeEmailModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToPage("/Account/Login");

        FirstName = user.FirstName;
        LastName = user.LastName;
        StudId = user.StudId;
        Specialization = user.Specialization;

        ChangeEmail = new ChangeEmailModel { Email = user.Email };
        ChangePassword = new ChangePasswordModel();

        return Page();
    }

    public async Task<IActionResult> OnPostChangeEmailAsync()
    {
        if (!ModelState.IsValid) return Page();

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToPage("/Account/Login");

        if (ChangeEmail.Email != user.Email)
        {
            var setEmailResult = await _userManager.SetEmailAsync(user, ChangeEmail.Email);
            if (!setEmailResult.Succeeded)
            {
                foreach (var error in setEmailResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }
            user.UserName = ChangeEmail.Email;
            await _userManager.UpdateAsync(user);
            StatusMessage = "Your email has been updated.";
            await _signInManager.RefreshSignInAsync(user);
        }
        else
        {
            StatusMessage = "No changes made.";
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostChangePasswordAsync()
    {
        if (!ModelState.IsValid) return Page();

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToPage("/Account/Login");

        var changePasswordResult = await _userManager.ChangePasswordAsync(user, ChangePassword.CurrentPassword, ChangePassword.NewPassword);
        if (!changePasswordResult.Succeeded)
        {
            foreach (var error in changePasswordResult.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return Page();
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "Your password has been changed.";

        // Clear password fields
        ChangePassword = new ChangePasswordModel();

        return RedirectToPage();
    }
}