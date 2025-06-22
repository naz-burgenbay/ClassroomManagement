using System;
using System.Collections.Generic;

namespace ClassroomManagement.Models
{
    public class HomeworkTask
    {
        public int Id { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public string InstructorId { get; set; }
        public ApplicationUser Instructor { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? FilePath { get; set; }

        public ICollection<HomeworkTaskFile> Files { get; set; } = new List<HomeworkTaskFile>();
        public ICollection<HomeworkSubmission> Submissions { get; set; } = new List<HomeworkSubmission>();
    }
}