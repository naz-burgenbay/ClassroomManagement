using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ClassroomManagement.Models;
using ClassroomManagement.Services;

namespace ClassroomManagement.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class EditCourseModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CourseService _courseService;

        public EditCourseModel(UserManager<ApplicationUser> userManager, CourseService courseService)
        {
            _userManager = userManager;
            _courseService = courseService;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }
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

        public async Task<IActionResult> OnGetAsync()
        {
            var course = await _courseService.GetCourseByIdAsync(Id);

            if (course == null) return NotFound();

            Name = course.Name;
            Description = course.Description;
            InstructorId = course.InstructorId;
            SelectedStudentIds = course.StudentCourse.Select(sc => sc.StudentId).ToList();

            var users = _userManager.Users.ToList();
            Instructors = new List<ApplicationUser>();
            Students = new List<ApplicationUser>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Instructor")) Instructors.Add(user);
                if (roles.Contains("Student")) Students.Add(user);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var updated = await _courseService.UpdateCourseAsync(Id, Name, Description, InstructorId, SelectedStudentIds);

            if (!updated) return NotFound();

            return RedirectToPage("/Admin/Courses");
        }
    }
}