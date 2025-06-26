using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ClassroomManagement.Data;
using ClassroomManagement.Models;

public class SubmissionModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public SubmissionModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public int Id { get; set; }

    public HomeworkSubmission Submission { get; set; }

    [BindProperty]
    public decimal? Grade { get; set; }
    [BindProperty]
    public string Feedback { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Submission = await _context.HomeworkSubmissions
            .Include(s => s.Student)
            .Include(s => s.Files)
            .FirstOrDefaultAsync(s => s.Id == Id);

        if (Submission == null) return NotFound();

        Grade = Submission.Grade;
        Feedback = Submission.Feedback;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var submission = await _context.HomeworkSubmissions.FindAsync(Id);
        if (submission == null) return NotFound();

        submission.Grade = Grade;
        submission.Feedback = Feedback;

        await _context.SaveChangesAsync();
        return RedirectToPage("/Homework", new { id = submission.HomeworkTaskId });
    }
}