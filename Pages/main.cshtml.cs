using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ClassroomManagement.Data;
using ClassroomManagement.Models;

public class DashboardModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public string UserName { get; set; }
    public List<NavItem> Navigation { get; set; }
    public List<CourseViewModel> Courses { get; set; } = new();
    public bool IsAdmin { get; set; }

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        UserName = user?.FirstName + " " + user?.LastName ?? user?.UserName ?? "User";

        var nav = new List<NavItem>();

        if (user != null)
        {
            var roles = await _userManager.GetRolesAsync(user);
            bool isAdmin = roles.Contains("Admin");

            nav.Add(new NavItem
            {
                Key = "myCourses",
                Label = isAdmin ? "Courses" : "My Courses",
                Url = "/Main"
            });

            nav.Add(new NavItem
            {
                Key = "profile",
                Label = "Profile",
                Url = "/Profile"
            });

            nav.Add(new NavItem
            {
                Key = "settings",
                Label = "Settings",
                Url = "/Settings"
            });

            if (isAdmin)
            {
                nav.Add(new NavItem
                {
                    Key = "userManagement",
                    Label = "User Management",
                    Url = "Admin/Users"
                });
                
                nav.Add(new NavItem
                {
                    Key = "courseManagement",
                    Label = "Course Management",
                    Url = "/Admin/Courses"
                });
            }

            if (isAdmin)
            {
                Courses = await _context.Courses
                    .Select(c => new CourseViewModel
                    {
                        Id = c.Id,
                        Title = c.Name,
                        Code = c.Description,
                        Image = ""
                    })
                    .ToListAsync();
            }
            else if (roles.Contains("Student"))
            {
                Courses = await _context.StudentCourses
                    .Where(sc => sc.StudentId == user.Id)
                    .Include(sc => sc.Course)
                    .Select(sc => new CourseViewModel
                    {
                        Id = sc.Course.Id,
                        Title = sc.Course.Name,
                        Code = sc.Course.Description,
                        Image = ""
                    })
                    .ToListAsync();
            }
            else if (roles.Contains("Instructor"))
            {
                Courses = await _context.Courses
                    .Where(c => c.InstructorId == user.Id)
                    .Select(c => new CourseViewModel
                    {
                        Id = c.Id,
                        Title = c.Name,
                        Code = c.Description,
                        Image = ""
                    })
                    .ToListAsync();
            }
        }

        Navigation = nav;
    }

    public class NavItem
    {
        public string Key { get; set; }
        public string Label { get; set; }
        public string Url { get; set; }
    }

    public class CourseViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public string Image { get; set; }
    }
}