namespace ClassroomManagement.Models;

public class CourseMaterial
{
    public int Id { get; set; }

    public int CourseId { get; set; }
    public Course Course { get; set; }

    public string InstructorId { get; set; }
    public ApplicationUser Instructor { get; set; }

    public string Title { get; set; }
    public string Content { get; set; }

    public string? FilePath { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<CourseMaterialFile> Files { get; set; }
}