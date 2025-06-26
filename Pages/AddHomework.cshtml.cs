using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassroomManagement.Data;
using ClassroomManagement.Models;
using Microsoft.AspNetCore.Identity;

public class AddHomeworkModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _env;

    public AddHomeworkModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
    {
        _context = context;
        _userManager = userManager;
        _env = env;
    }

    [BindProperty(SupportsGet = true)]
    public int CourseId { get; set; }

    [BindProperty]
    public string Title { get; set; }
    [BindProperty]
    public string Description { get; set; }
    [BindProperty]
    public DateTime DueDate { get; set; }

    [BindProperty]
    public List<IFormFile> Uploads { get; set; } = new List<IFormFile>();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var instructorId = _userManager.GetUserId(User);

        var task = new HomeworkTask
        {
            CourseId = CourseId,
            Title = Title,
            Description = Description,
            DueDate = DueDate,
            InstructorId = instructorId,
            Files = new List<HomeworkTaskFile>()
        };

        _context.HomeworkTasks.Add(task);
        await _context.SaveChangesAsync();

        if (Uploads != null && Uploads.Any())
        {
            var uploadFolder = Path.Combine(_env.WebRootPath, "uploads", "homeworktasks", task.Id.ToString());
            Directory.CreateDirectory(uploadFolder);

            foreach (var file in Uploads)
            {
                if (file.Length > 0)
                {
                    var uniqueName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(uploadFolder, uniqueName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var relPath = $"/uploads/homeworktasks/{task.Id}/{uniqueName}";
                    task.Files.Add(new HomeworkTaskFile
                    {
                        FileName = file.FileName,
                        FilePath = relPath,
                        ContentType = file.ContentType
                    });
                }
            }
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("/Course", new { id = CourseId });
    }
}