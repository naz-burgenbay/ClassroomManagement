using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassroomManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ClassroomManagement.Services;

namespace ClassroomManagement.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class CreateCourseModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CourseService _courseService;

        public CreateCourseModel(UserManager<ApplicationUser> userManager, CourseService courseService)
        {
            _userManager = userManager;
            _courseService = courseService;
        }

        [BindProperty]
        public string Name { get; set; }
        [BindProperty]
        public string Description { get; set; }
        [BindProperty]
        public string InstructorId { get; set; }
        [BindProperty]
        public List<string> SelectedStudentIds { get; set; } = new();

        public List<ApplicationUser> Instructors { get; set; }
        public List<ApplicationUser> Students { get; set; }

        public async Task OnGetAsync()
        {
            var users = _userManager.Users.ToList();
            Instructors = new List<ApplicationUser>();
            Students = new List<ApplicationUser>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Instructor")) Instructors.Add(user);
                if (roles.Contains("Student")) Students.Add(user);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await OnGetAsync();

            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Description) || string.IsNullOrWhiteSpace(InstructorId))
            {
                ModelState.AddModelError("", "All fields required.");
                return Page();
            }

            await _courseService.CreateCourseAsync(Name, Description, InstructorId, SelectedStudentIds);

            return RedirectToPage("/Admin/Courses");
        }
    }
}