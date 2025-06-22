using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClassroomManagement.Data;
using ClassroomManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClassroomManagement.Models;
using ClassroomManagement.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ClassroomManagement.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class CoursesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CoursesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Course> Courses { get; set; }

        public async Task OnGetAsync()
        {
            Courses = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.StudentCourse)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var course = await _context.Courses
                .Include(c => c.StudentCourse)
                .Include(c => c.Materials)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course != null)
            {
                _context.CourseMaterials.RemoveRange(course.Materials);

                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}