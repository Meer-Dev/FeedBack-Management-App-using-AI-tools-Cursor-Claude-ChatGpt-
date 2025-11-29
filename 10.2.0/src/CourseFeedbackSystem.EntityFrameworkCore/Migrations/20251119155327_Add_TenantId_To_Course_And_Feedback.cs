using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseFeedbackSystem.Migrations
{
    /// <inheritdoc />
    public partial class Add_TenantId_To_Course_And_Feedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Feedbacks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Courses");
        }
    }
}
