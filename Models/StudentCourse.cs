namespace ClassroomManagement.Models;

public class StudentCourse
{
    public string StudentId { get; set; }
    public ApplicationUser Student { get; set; } = null!;

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
}
