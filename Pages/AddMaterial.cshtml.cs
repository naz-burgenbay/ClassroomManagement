using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassroomManagement.Data;
using ClassroomManagement.Models;
using Microsoft.AspNetCore.Identity;

public class AddMaterialModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _env;

    public AddMaterialModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
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
    public string Content { get; set; }

    // Allow multiple files
    [BindProperty]
    public List<IFormFile> Uploads { get; set; } = new List<IFormFile>();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var instructorId = _userManager.GetUserId(User);

        var material = new CourseMaterial
        {
            CourseId = CourseId,
            Title = Title,
            Content = Content,
            InstructorId = instructorId,
            Files = new List<CourseMaterialFile>()
        };

        // Save material first to generate ID for file association
        _context.CourseMaterials.Add(material);
        await _context.SaveChangesAsync();

        if (Uploads != null && Uploads.Any())
        {
            var uploadFolder = Path.Combine(_env.WebRootPath, "uploads", "coursematerials", material.Id.ToString());
            Directory.CreateDirectory(uploadFolder);

            foreach (var file in Uploads)
            {
                if (file.Length > 0)
                {
                    var uniqueName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{System.Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(uploadFolder, uniqueName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var relPath = $"/uploads/coursematerials/{material.Id}/{uniqueName}";
                    material.Files.Add(new CourseMaterialFile
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