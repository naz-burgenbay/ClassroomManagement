using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClassroomManagement.Data;
using ClassroomManagement.Models;

public class CourseModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CourseModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string InstructorName { get; set; } = "";
    public List<string> StudentNames { get; set; } = new();
    public List<CourseMaterial> Materials { get; set; } = new();
    public List<HomeworkTask> Homeworks { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var course = await _context.Courses
            .Include(c => c.Instructor)
            .Include(c => c.StudentCourse).ThenInclude(sc => sc.Student)
            .Include(c => c.Materials).ThenInclude(m => m.Files)
            .Include(c => c.HomeworkTasks).ThenInclude(h => h.Files)
            .FirstOrDefaultAsync(c => c.Id == Id);

        if (course == null)
        {
            return NotFound();
        }

        Name = course.Name;
        Description = course.Description;
        InstructorName = course.Instructor?.FirstName + " " + course.Instructor?.LastName;
        StudentNames = course.StudentCourse.Select(sc => sc.Student.FirstName + " " + sc.Student.LastName).ToList();
        Materials = course.Materials.ToList();
        Homeworks = course.HomeworkTasks.OrderByDescending(h => h.DueDate).ToList();

        return Page();
    }
}