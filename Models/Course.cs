namespace ClassroomManagement.Models;

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string InstructorId { get; set; }
    public ApplicationUser Instructor { get; set; } = null!;

    public string Description { get; set; }

    public List<StudentCourse> StudentCourse { get; set; } = new();
    public ICollection<CourseMaterial> Materials { get; set; } = new List<CourseMaterial>();
    public ICollection<HomeworkTask> HomeworkTasks { get; set; } = new List<HomeworkTask>();

}
