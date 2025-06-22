using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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
    public class EditCourseModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditCourseModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            var course = await _context.Courses
                .Include(c => c.StudentCourse)
                .FirstOrDefaultAsync(c => c.Id == Id);

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
            var course = await _context.Courses
                .Include(c => c.StudentCourse)
                .FirstOrDefaultAsync(c => c.Id == Id);

            if (course == null) return NotFound();

            course.Name = Name;
            course.Description = Description;
            course.InstructorId = InstructorId;

            _context.StudentCourses.RemoveRange(course.StudentCourse);
            if (SelectedStudentIds != null && SelectedStudentIds.Any())
            {
                course.StudentCourse = SelectedStudentIds.Select(sid => new StudentCourse
                {
                    StudentId = sid,
                    CourseId = course.Id
                }).ToList();
            }
            else
            {
                course.StudentCourse = new List<StudentCourse>();
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("/Admin/Courses");
        }
    }
}