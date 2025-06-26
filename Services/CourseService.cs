using ClassroomManagement.Data;
using ClassroomManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassroomManagement.Services
{
    public class CourseService
    {
        private readonly ApplicationDbContext _context;

        public CourseService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all courses with instructors and students
        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.StudentCourse)
                .ToListAsync();
        }

        // Create a new course with assigned instructor and students
        public async Task<Course> CreateCourseAsync(string name, string description, string instructorId, List<string> studentIds)
        {
            var course = new Course
            {
                Name = name,
                Description = description,
                InstructorId = instructorId,
                StudentCourse = studentIds?.Select(sid => new StudentCourse
                {
                    StudentId = sid
                }).ToList() ?? new List<StudentCourse>()
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        // Get a course by ID (for edit)
        public async Task<Course> GetCourseByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.StudentCourse)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // Update an existing course and its students/instructor
        public async Task<bool> UpdateCourseAsync(int courseId, string name, string description, string instructorId, List<string> studentIds)
        {
            var course = await _context.Courses
                .Include(c => c.StudentCourse)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null) return false;

            course.Name = name;
            course.Description = description;
            course.InstructorId = instructorId;

            // Update students
            _context.StudentCourses.RemoveRange(course.StudentCourse);

            if (studentIds != null && studentIds.Any())
            {
                course.StudentCourse = studentIds.Select(sid => new StudentCourse
                {
                    StudentId = sid,
                    CourseId = course.Id
                }).ToList();
            }
            else
            {
                course.StudentCourse = new List<StudentCourse>();
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // Delete a course and its materials
        public async Task<bool> DeleteCourseAsync(int id)
        {
            var course = await _context.Courses
                .Include(c => c.StudentCourse)
                .Include(c => c.Materials)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null) return false;

            _context.CourseMaterials.RemoveRange(course.Materials);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}