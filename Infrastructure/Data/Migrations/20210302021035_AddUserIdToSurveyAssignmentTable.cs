using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddUserIdToSurveyAssignmentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "SurveyAssignments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyAssignments_UserId",
                table: "SurveyAssignments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyAssignments_AspNetUsers_UserId",
                table: "SurveyAssignments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SurveyAssignments_AspNetUsers_UserId",
                table: "SurveyAssignments");

            migrationBuilder.DropIndex(
                name: "IX_SurveyAssignments_UserId",
                table: "SurveyAssignments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SurveyAssignments");
        }
    }
}
