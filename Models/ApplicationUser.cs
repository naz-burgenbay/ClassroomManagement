using Microsoft.AspNetCore.Identity;

namespace ClassroomManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? StudId { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string? Specialization { get; set; }

        public List<Course>? CoursesTaught { get; set; }
        public List<StudentCourse>? StudentCourses { get; set; }
    }
}
