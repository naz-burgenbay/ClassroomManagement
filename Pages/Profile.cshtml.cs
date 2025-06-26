using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassroomManagement.Models;

[Authorize]
public class ProfileModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string StudId { get; set; }
    public string Specialization { get; set; }
    public string Email { get; set; }

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            Response.Redirect("/Account/Login");
            return;
        }
        FirstName = user.FirstName;
        LastName = user.LastName;
        StudId = user.StudId;
        Specialization = user.Specialization;
        Email = user.Email;
    }
}