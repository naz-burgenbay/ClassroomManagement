using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using ClassroomManagement.Models;
using ClassroomManagement.Services;

namespace ClassroomManagement.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class CoursesModel : PageModel
    {
        private readonly CourseService _courseService;

        public CoursesModel(CourseService courseService)
        {
            _courseService = courseService;
        }

        public List<Course> Courses { get; set; }

        public async Task OnGetAsync()
        {
            Courses = await _courseService.GetAllCoursesAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _courseService.DeleteCourseAsync(id);
            return RedirectToPage();
        }
    }
}