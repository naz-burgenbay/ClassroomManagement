// using ClassroomManagement.Models;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.EntityFrameworkCore;
// using System;
// using System.Linq;
// using System.Threading.Tasks;

// namespace ClassroomManagement.Data
// {
//     public static class DataSeeder
//     {
//         public static async Task SeedStudents(UserManager<ApplicationUser> userManager)
//         {
//             var students = new[]
//             {
//                 new { User = new ApplicationUser
//                         {
//                             StudId = "w69645",
//                             UserName = "gulzatyessen@gmail.com",
//                             Email = "gulzatyessen@gmail.com",
//                             FirstName = "Gulzat",
//                             LastName = "Yessen",
//                             EmailConfirmed = true
//                         },
//                     Password = "Gulzatpswd1"
//                 },
//                 new { User = new ApplicationUser
//                         {
//                             StudId = "w70963",
//                             UserName = "nazburgenbay01@gmail.com",
//                             Email = "nazburgenbay01@gmail.com",
//                             FirstName = "Naz",
//                             LastName = "Burgenbay",
//                             EmailConfirmed = true
//                         },
//                     Password = "Nazpswd1"
//                 },
//                 new { User = new ApplicationUser
//                         {
//                             StudId = "w69646",
//                             UserName = "dilnazdosumbek7@gmail.com",
//                             Email = "dilnazdosumbek7@gmail.com",
//                             FirstName = "Dilnaz",
//                             LastName = "Dosumbek",
//                             EmailConfirmed = true
//                         },
//                     Password = "Dilnazpswd1"
//                 }
//             };

//             foreach (var studentData in students)
//             {
//                 var user = await userManager.FindByEmailAsync(studentData.User.Email);
//                 if (user == null)
//                 {
//                     var result = await userManager.CreateAsync(studentData.User, studentData.Password);
//                     if (result.Succeeded)
//                     {
//                         await userManager.AddToRoleAsync(studentData.User, "Student");
//                     }
//                     else
//                     {
//                         Console.WriteLine($"Failed to create student {studentData.User.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
//                     }
//                 }
//             }
//         }

//         public static async Task SeedInstructors(UserManager<ApplicationUser> userManager)
//         {
//             var instructors = new[]
//             {
//                 new { User = new ApplicationUser
//                         {
//                             UserName = "mjagiela@classroom.com",
//                             Email = "mjagiela@classroom.com",
//                             FirstName = "Marcin",
//                             LastName = "Jagiela",
//                             EmailConfirmed = true
//                         },
//                       Password = "Mjagielapswd1"
//                 },
//                 new { User = new ApplicationUser
//                         {
//                             UserName = "mknap@classroom.com",
//                             Email = "mknap@classroom.com",
//                             FirstName = "Maksymilian",
//                             LastName = "Knap",
//                             EmailConfirmed = true
//                         },
//                       Password = "Mknappswd1"
//                 }
//             };

//             foreach (var instructorData in instructors)
//             {
//                 var user = await userManager.FindByEmailAsync(instructorData.User.Email);
//                 if (user == null)
//                 {
//                     var result = await userManager.CreateAsync(instructorData.User, instructorData.Password);
//                     if (result.Succeeded)
//                     {
//                         await userManager.AddToRoleAsync(instructorData.User, "Instructor");
//                     }
//                     else
//                     {
//                         Console.WriteLine($"Failed to create user {instructorData.User.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
//                     }
//                 }
//             }
//         }

//         public static async Task SeedCourses(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
//         {
//             // await context.Database.EnsureCreatedAsync();

//             // Get instructors seeded earlier
//             var instructor1 = await userManager.Users.FirstOrDefaultAsync(u => u.Email == "mjagiela@classroom.com");
//             var instructor2 = await userManager.Users.FirstOrDefaultAsync(u => u.Email == "mknap@classroom.com");

//             if (instructor1 == null || instructor2 == null)
//             {
//                 throw new Exception("Instructors must be seeded before courses.");
//             }

//             if (!context.Courses.Any())
//             {
//                 var courses = new[]
//                 {
//                     new Course
//                     {
//                         Name = "Programming Languages",
//                         InstructorId = instructor1.Id,
//                         Description = "Basics of .NET"
//                     },
//                     new Course
//                     {
//                         Name = "Software Engineering",
//                         InstructorId = instructor2.Id,
//                         Description = "All about the Software Development cycle"
//                     }
//                 };

//                 context.Courses.AddRange(courses);
//                 await context.SaveChangesAsync();
//             }

//             if (!context.CourseMaterials.Any())
//             {
//                 var introCourse = await context.Courses.FirstOrDefaultAsync(c => c.Name == "Programming Languages");
//                 var algoCourse = await context.Courses.FirstOrDefaultAsync(c => c.Name == "Software Engineering");

//                 var materials = new[]
//                 {
//                     new CourseMaterial
//                     {
//                         CourseId = introCourse.Id,
//                         InstructorId = instructor1.Id,
//                         Title = "Lecture 1: Getting Started",
//                         Content = "Welcome to programming!",
//                         FilePath = null
//                     },
//                     new CourseMaterial
//                     {
//                         CourseId = algoCourse.Id,
//                         InstructorId = instructor2.Id,
//                         Title = "Lecture 1: Types of requirements",
//                         Content = "Will add .pdf file soon",
//                         FilePath = null
//                     }
//                 };

//                 context.CourseMaterials.AddRange(materials);
//                 await context.SaveChangesAsync();
//             }
//         }

//         public static async Task SeedStudentCourses(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
//         {
//             // await context.Database.EnsureCreatedAsync();

//             var student1 = await userManager.Users.FirstOrDefaultAsync(u => u.Email == "gulzatyessen@gmail.com");
//             var student2 = await userManager.Users.FirstOrDefaultAsync(u => u.Email == "nazburgenbay01@gmail.com");
//             var student3 = await userManager.Users.FirstOrDefaultAsync(u => u.Email == "dilnazdosumbek7@gmail.com");

//             if (student1 == null || student2 == null || student3 == null)
//             {
//                 throw new Exception("Students must be seeded before assigning courses.");
//             }

//             var course1 = await context.Courses.FirstOrDefaultAsync(c => c.Name == "Programming Languages");
//             var course2 = await context.Courses.FirstOrDefaultAsync(c => c.Name == "Software Engineering");

//             if (course1 == null || course2 == null)
//             {
//                 throw new Exception("Courses must be seeded before assigning students.");
//             }

//             if (!context.StudentCourses.Any())
//             {
//                 var studentCourses = new[]
//                 {
//                     new StudentCourse { StudentId = student1.Id, CourseId = course1.Id },
//                     new StudentCourse { StudentId = student1.Id, CourseId = course2.Id },
//                     new StudentCourse { StudentId = student2.Id, CourseId = course1.Id },
//                     new StudentCourse { StudentId = student2.Id, CourseId = course2.Id },
//                     new StudentCourse { StudentId = student3.Id, CourseId = course1.Id },
//                     new StudentCourse { StudentId = student3.Id, CourseId = course2.Id },
//                 };

//                 context.StudentCourses.AddRange(studentCourses);
//                 await context.SaveChangesAsync();
//             }
//         }
//     }
// }