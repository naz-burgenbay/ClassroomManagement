using ClassroomManagement.Data;
using ClassroomManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ClassroomManagement.Services;
using DotNetEnv;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

if (File.Exists("secret.env"))
{
    Env.Load("secret.env");
}

builder.Services.AddRazorPages();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Only register DbContext and Identity if connection string is configured
if (!string.IsNullOrEmpty(connectionString))
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));

    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
    })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    builder.Services.AddScoped<HomeworkSubmissionService>();
    builder.Services.AddScoped<UserService>();
    builder.Services.AddScoped<CourseService>();

    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Identity/Logout";
    });
}
else
{
    // Add dummy stores for Identity without database
    builder.Services.AddSingleton(typeof(IUserStore<>), typeof(NullUserStore<>));
    builder.Services.AddSingleton(typeof(IRoleStore<>), typeof(NullRoleStore<>));
    
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
    })
        .AddDefaultTokenProviders();

    builder.Services.AddScoped<SignInManager<ApplicationUser>>();
    builder.Services.AddScoped<UserManager<ApplicationUser>>();
}

var app = builder.Build();

// TODO: Uncomment when Azure SQL is configured
// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     var context = services.GetRequiredService<ApplicationDbContext>();
//     var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
//     var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
//
//     await DbInitializer.Initialize(context, userManager, roleManager);
//
//     await DataSeeder.SeedStudents(userManager);
//     await DataSeeder.SeedInstructors(userManager);
//     await DataSeeder.SeedCourses(context, userManager);
//     await DataSeeder.SeedStudentCourses(context, userManager);
// }

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
