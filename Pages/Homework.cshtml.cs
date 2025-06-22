using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ClassroomManagement.Data;
using ClassroomManagement.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System;

public class HomeworkModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _env;

    public HomeworkModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
    {
        _context = context;
        _userManager = userManager;
        _env = env;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool EditMode { get; set; } = false;

    public HomeworkTask Homework { get; set; }

    [BindProperty]
    [Required]
    public string Title { get; set; }

    [BindProperty]
    [Required]
    public string Description { get; set; }

    [BindProperty]
    [Required]
    public DateTime DueDate { get; set; }

    [BindProperty]
    public List<IFormFile> NewFiles { get; set; } = new();

    [BindProperty]
    public List<int> FilesToDelete { get; set; } = new();

    [BindProperty]
    public string AnswerText { get; set; }
    [BindProperty]
    public List<IFormFile> SubmissionFiles { get; set; } = new();

    public HomeworkSubmission Submission { get; set; }
    public List<HomeworkSubmission> Submissions { get; set; } = new();

    public bool IsInstructor { get; set; }
    public bool IsStudent { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Homework = await _context.HomeworkTasks
            .Include(h => h.Files)
            .Include(h => h.Course)
            .FirstOrDefaultAsync(h => h.Id == Id);

        if (Homework == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);
        IsInstructor = User.IsInRole("Instructor") && user?.Id == Homework.InstructorId;
        IsStudent = User.IsInRole("Student");

        Title = Homework.Title;
        Description = Homework.Description;
        DueDate = Homework.DueDate;

        if (IsStudent && user != null)
        {
            Submission = await _context.HomeworkSubmissions
                .Include(s => s.Files)
                .FirstOrDefaultAsync(s => s.HomeworkTaskId == Id && s.StudentId == user.Id);
        }

        if (IsInstructor)
        {
            Submissions = await _context.HomeworkSubmissions
                .Include(s => s.Student)
                .Where(s => s.HomeworkTaskId == Id)
                .ToListAsync();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Homework = await _context.HomeworkTasks
            .Include(h => h.Files)
            .FirstOrDefaultAsync(h => h.Id == Id);

        if (Homework == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);
        IsInstructor = User.IsInRole("Instructor") && user?.Id == Homework.InstructorId;
        if (!IsInstructor)
            return Forbid();

        if (!ModelState.IsValid)
        {
            EditMode = true;
            return Page();
        }

        Homework.Title = Title;
        Homework.Description = Description;
        Homework.DueDate = DueDate;

        if (FilesToDelete != null && FilesToDelete.Any())
        {
            var filesToRemove = Homework.Files.Where(f => FilesToDelete.Contains(f.Id)).ToList();
            foreach (var file in filesToRemove)
            {
                var absolutePath = Path.Combine(_env.WebRootPath, file.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(absolutePath))
                    System.IO.File.Delete(absolutePath);

                _context.Set<HomeworkTaskFile>().Remove(file);
            }
        }

        if (NewFiles != null && NewFiles.Any())
        {
            var uploadFolder = Path.Combine(_env.WebRootPath, "uploads", "homeworktasks", Homework.Id.ToString());
            Directory.CreateDirectory(uploadFolder);

            foreach (var file in NewFiles)
            {
                if (file.Length > 0)
                {
                    var uniqueName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(uploadFolder, uniqueName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var relPath = $"/uploads/homeworktasks/{Homework.Id}/{uniqueName}";
                    Homework.Files.Add(new HomeworkTaskFile
                    {
                        FileName = file.FileName,
                        FilePath = relPath,
                        ContentType = file.ContentType
                    });
                }
            }
        }

        await _context.SaveChangesAsync();

        return RedirectToPage("/Homework", new { id = Homework.Id });
    }

    public async Task<IActionResult> OnPostDeleteAsync()
    {
        var homework = await _context.HomeworkTasks.FindAsync(Id);
        if (homework == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);
        var isInstructor = User.IsInRole("Instructor") && user?.Id == homework.InstructorId;
        if (!isInstructor)
            return Forbid();

        var courseId = homework.CourseId;
        _context.HomeworkTasks.Remove(homework);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Course", new { id = courseId });
    }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null || !User.IsInRole("Student"))
            return Forbid();

        var homeworkTask = await _context.HomeworkTasks.FindAsync(Id);
        if (homeworkTask == null)
        {
            ModelState.AddModelError("", "The homework task does not exist.");
            await OnGetAsync();
            return Page();
        }

        var existing = await _context.HomeworkSubmissions
            .FirstOrDefaultAsync(s => s.HomeworkTaskId == homeworkTask.Id && s.StudentId == user.Id);
        if (existing != null)
            return RedirectToPage(new { id = Id });

        var submission = new HomeworkSubmission
        {
            HomeworkTaskId = homeworkTask.Id,
            StudentId = user.Id,
            AnswerText = AnswerText,
            SubmittedAt = DateTime.UtcNow,
            Files = new List<HomeworkSubmissionFile>()
        };

        _context.HomeworkSubmissions.Add(submission);
        await _context.SaveChangesAsync();

        if (SubmissionFiles != null && SubmissionFiles.Any())
        {
            var uploadFolder = Path.Combine(_env.WebRootPath, "uploads", "homeworksubmissions", submission.Id.ToString());
            Directory.CreateDirectory(uploadFolder);

            foreach (var file in SubmissionFiles)
            {
                if (file.Length > 0)
                {
                    var uniqueName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(uploadFolder, uniqueName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var relPath = $"/uploads/homeworksubmissions/{submission.Id}/{uniqueName}";
                    submission.Files.Add(new HomeworkSubmissionFile
                    {
                        FileName = file.FileName,
                        FilePath = relPath,
                        ContentType = file.ContentType
                    });
                }
            }
            await _context.SaveChangesAsync();
        }

        return RedirectToPage(new { id = Id });
    }
}