using System;

namespace ClassroomManagement.Models
{
    public class HomeworkSubmission
    {
        public int Id { get; set; }

        public int HomeworkTaskId { get; set; }
        public HomeworkTask HomeworkTask { get; set; }

        public string StudentId { get; set; }
        public ApplicationUser Student { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public string? AnswerText { get; set; }
        public string? FilePath { get; set; }
        public decimal? Grade { get; set; }

        public string? Feedback { get; set; }
        public ICollection<HomeworkSubmissionFile> Files { get; set; } = new List<HomeworkSubmissionFile>();
    }
}