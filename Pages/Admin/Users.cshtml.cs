using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassroomManagement.Models;
using ClassroomManagement.Services;
using ClassroomManagement.Data;

namespace ClassroomManagement.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UsersModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;

        public List<ApplicationUser> Instructors { get; set; }
        public List<ApplicationUser> Students { get; set; }

        public UsersModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            UserService userService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _userService = userService;
        }

        [BindProperty]
        public string NewEmail { get; set; }
        [BindProperty]
        public string NewFirstName { get; set; }
        [BindProperty]
        public string NewLastName { get; set; }
        [BindProperty]
        public string NewPassword { get; set; }
        [BindProperty]
        public string NewRole { get; set; }
        [BindProperty]
        public string NewStudId { get; set; }
        [BindProperty]
        public string NewSpecialization { get; set; }

        [BindProperty]
        public string EditUserId { get; set; }
        [BindProperty]
        public string EditEmail { get; set; }
        [BindProperty]
        public string EditFirstName { get; set; }
        [BindProperty]
        public string EditLastName { get; set; }
        [BindProperty]
        public string EditStudId { get; set; }
        [BindProperty]
        public string EditSpecialization { get; set; }

        public async Task OnGetAsync()
        {
            var users = _userManager.Users.ToList();

            Instructors = new List<ApplicationUser>();
            Students = new List<ApplicationUser>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (_userService.IsInstructor(roles)) Instructors.Add(user);
                if (_userService.IsStudent(roles)) Students.Add(user);
            }
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            var user = new ApplicationUser
            {
                UserName = NewEmail,
                Email = NewEmail,
                FirstName = NewFirstName,
                LastName = NewLastName,
                EmailConfirmed = true
            };

            if (NewRole == "Student")
            {
                if (string.IsNullOrWhiteSpace(NewStudId))
                {
                    ModelState.AddModelError("NewStudId", "Student ID is required for students.");
                    await OnGetAsync();
                    return Page();
                }
                if (!_userService.IsValidStudentId(NewStudId))
                {
                    ModelState.AddModelError("NewStudId", "Student ID must start with 'w' and have 5 digits.");
                    await OnGetAsync();
                    return Page();
                }
                if (_userManager.Users.Any(u => u.StudId == NewStudId))
                {
                    ModelState.AddModelError("NewStudId", "Student ID already exists.");
                    await OnGetAsync();
                    return Page();
                }
                user.StudId = NewStudId;
                user.Specialization = null;
            }
            else
            {
                user.StudId = null;
                user.Specialization = NewSpecialization;
            }

            var result = await _userManager.CreateAsync(user, NewPassword);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, NewRole);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                await OnGetAsync();
                return Page();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return RedirectToPage();

            EditUserId = user.Id;
            EditEmail = user.Email;
            EditFirstName = user.FirstName;
            EditLastName = user.LastName;
            EditStudId = user.StudId;
            EditSpecialization = user.Specialization;
            await OnGetAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            var user = await _userManager.FindByIdAsync(EditUserId);
            if (user == null)
                return RedirectToPage();

            user.Email = EditEmail;
            user.UserName = EditEmail;
            user.FirstName = EditFirstName;
            user.LastName = EditLastName;

            var roles = await _userManager.GetRolesAsync(user);
            if (_userService.IsStudent(roles))
            {
                if (!_userService.IsValidStudentId(EditStudId))
                {
                    ModelState.AddModelError("EditStudId", "Student ID must start with 'w' and have 5 digits.");
                    await OnGetAsync();
                    return Page();
                }
                if (_userManager.Users.Any(u => u.StudId == EditStudId && u.Id != EditUserId))
                {
                    ModelState.AddModelError("EditStudId", "Student ID already exists.");
                    await OnGetAsync();
                    return Page();
                }
                user.StudId = EditStudId;
                user.Specialization = null;
            }
            else
            {
                user.StudId = null;
                user.Specialization = EditSpecialization;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                await OnGetAsync();
                return Page();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (_userService.IsStudent(roles))
                {
                    var studentCourses = _context.StudentCourses.Where(sc => sc.StudentId == user.Id);
                    _context.StudentCourses.RemoveRange(studentCourses);
                    await _context.SaveChangesAsync();
                }

                await _userManager.DeleteAsync(user);
            }
            return RedirectToPage();
        }
    }
}