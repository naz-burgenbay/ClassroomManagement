namespace ClassroomManagement.Models;

public class CourseMaterialFile
{
    public int Id { get; set; }

    public int CourseMaterialId { get; set; }
    public CourseMaterial CourseMaterial { get; set; }

    public string FileName { get; set; }
    public string FilePath { get; set; }
    public string ContentType { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}