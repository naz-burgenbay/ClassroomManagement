using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassroomManagement.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadePaths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HomeworkTasks_Courses_CourseId",
                table: "HomeworkTasks");

            migrationBuilder.AddForeignKey(
                name: "FK_HomeworkTasks_Courses_CourseId",
                table: "HomeworkTasks",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HomeworkTasks_Courses_CourseId",
                table: "HomeworkTasks");

            migrationBuilder.AddForeignKey(
                name: "FK_HomeworkTasks_Courses_CourseId",
                table: "HomeworkTasks",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
