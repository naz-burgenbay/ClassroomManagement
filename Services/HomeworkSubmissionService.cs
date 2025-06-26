using ClassroomManagement.Data;
using ClassroomManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassroomManagement.Services
{
    public class HomeworkSubmissionResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public HomeworkSubmission Submission { get; set; }
    }

    public class HomeworkSubmissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public HomeworkSubmissionService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<HomeworkSubmissionResult> SubmitHomeworkAsync(
            int homeworkTaskId,
            string studentId,
            string answerText,
            List<IFormFile> submissionFiles)
        {
            var homeworkTask = await _context.HomeworkTasks.FindAsync(homeworkTaskId);
            if (homeworkTask == null)
                return new HomeworkSubmissionResult { Success = false, ErrorMessage = "The homework task does not exist." };

            var existing = await _context.HomeworkSubmissions
                .FirstOrDefaultAsync(s => s.HomeworkTaskId == homeworkTask.Id && s.StudentId == studentId);
            if (existing != null)
                return new HomeworkSubmissionResult { Success = false, ErrorMessage = "You have already submitted this homework." };

            var submission = new HomeworkSubmission
            {
                HomeworkTaskId = homeworkTask.Id,
                StudentId = studentId,
                AnswerText = answerText,
                SubmittedAt = DateTime.UtcNow,
                Files = new List<HomeworkSubmissionFile>()
            };

            _context.HomeworkSubmissions.Add(submission);
            await _context.SaveChangesAsync();

            if (submissionFiles != null && submissionFiles.Any())
            {
                var uploadFolder = Path.Combine(_env.WebRootPath, "uploads", "homeworksubmissions", submission.Id.ToString());
                Directory.CreateDirectory(uploadFolder);

                foreach (var file in submissionFiles)
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

            return new HomeworkSubmissionResult { Success = true, Submission = submission };
        }
    }
}