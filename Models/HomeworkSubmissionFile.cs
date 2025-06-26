namespace ClassroomManagement.Models;

public class HomeworkSubmissionFile
{
    public int Id { get; set; }

    public int HomeworkSubmissionId { get; set; }
    public HomeworkSubmission HomeworkSubmission { get; set; }

    public string FileName { get; set; }
    public string FilePath { get; set; }
    public string ContentType { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
