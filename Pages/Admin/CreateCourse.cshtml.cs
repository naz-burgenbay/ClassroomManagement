using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassroomManagement.Models;
using ClassroomManagement.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ClassroomManagement.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class CreateCourseModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateCourseModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

            var course = new Course
            {
                Name = Name,
                Description = Description,
                InstructorId = InstructorId,
            };

            if (SelectedStudentIds != null && SelectedStudentIds.Any())
            {
                course.StudentCourse = SelectedStudentIds.Select(sid => new StudentCourse
                {
                    StudentId = sid
                }).ToList();
            }

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Admin/Courses");
        }
    }
}