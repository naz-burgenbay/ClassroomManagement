using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ClassroomManagement.Data;
using ClassroomManagement.Models;
using System.ComponentModel.DataAnnotations;

public class MaterialModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _env;

    public MaterialModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
    {
        _context = context;
        _userManager = userManager;
        _env = env;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool EditMode { get; set; } = false;

    public CourseMaterial? Material { get; set; }

    [BindProperty]
    [Required]
    public string Title { get; set; } = "";

    [BindProperty]
    [Required]
    public string Content { get; set; } = "";

    [BindProperty]
    public List<IFormFile> NewFiles { get; set; } = new List<IFormFile>();

    [BindProperty]
    public List<int> FilesToDelete { get; set; } = new List<int>();

    public bool IsInstructor { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Material = await _context.CourseMaterials
            .Include(m => m.Files)
            .Include(m => m.Course)
            .FirstOrDefaultAsync(m => m.Id == Id);

        if (Material == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);
        IsInstructor = User.IsInRole("Instructor") && user?.Id == Material.InstructorId;

        Title = Material.Title;
        Content = Material.Content;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var material = await _context.CourseMaterials
            .Include(m => m.Files)
            .FirstOrDefaultAsync(m => m.Id == Id);
        if (material == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);
        var isInstructor = User.IsInRole("Instructor") && user?.Id == material.InstructorId;
        if (!isInstructor)
            return Forbid();

        if (!ModelState.IsValid)
        {
            Material = material;
            IsInstructor = isInstructor;
            EditMode = true;
            return Page();
        }

        material.Title = Title;
        material.Content = Content;

        // === Delete selected files (checkbox version) ===
        if (FilesToDelete != null && FilesToDelete.Any())
        {
            var filesToRemove = material.Files.Where(f => FilesToDelete.Contains(f.Id)).ToList();
            foreach (var file in filesToRemove)
            {
                var absolutePath = Path.Combine(_env.WebRootPath, file.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(absolutePath))
                    System.IO.File.Delete(absolutePath);

                _context.CourseMaterialFiles.Remove(file);
            }
        }

        // === New file upload logic ===
        if (NewFiles != null && NewFiles.Any())
        {
            var uploadFolder = Path.Combine(_env.WebRootPath, "uploads", "coursematerials", material.Id.ToString());
            Directory.CreateDirectory(uploadFolder);

            foreach (var file in NewFiles)
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
        }

        await _context.SaveChangesAsync();

        return RedirectToPage("/Material", new { id = Id });
    }

    public async Task<IActionResult> OnPostDeleteAsync()
    {
        var material = await _context.CourseMaterials.FindAsync(Id);
        if (material == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);
        var isInstructor = User.IsInRole("Instructor") && user?.Id == material.InstructorId;
        if (!isInstructor)
            return Forbid();

        var courseId = material.CourseId;
        _context.CourseMaterials.Remove(material);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Course", new { id = courseId });
    }
}