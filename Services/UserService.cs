using System.Text.RegularExpressions;

namespace ClassroomManagement.Services
{
    public class UserService
    {
        public bool IsValidStudentId(string studId)
            => !string.IsNullOrWhiteSpace(studId) && Regex.IsMatch(studId, @"^w\d{5}$");

        public bool IsAdmin(IEnumerable<string> roles)
            => roles.Contains("Admin");

        public bool IsStudent(IEnumerable<string> roles)
            => roles.Contains("Student");

        public bool IsInstructor(IEnumerable<string> roles)
            => roles.Contains("Instructor");
    }
}