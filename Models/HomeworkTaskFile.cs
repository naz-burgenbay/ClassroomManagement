namespace ClassroomManagement.Models;

public class HomeworkTaskFile
{
    public int Id { get; set; }

    public int HomeworkTaskId { get; set; }
    public HomeworkTask HomeworkTask { get; set; }

    public string FileName { get; set; }
    public string FilePath { get; set; }
    public string ContentType { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}