using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F54 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DotKhamSteps_AspNetUsers_UserId",
                table: "DotKhamSteps");

            migrationBuilder.DropIndex(
                name: "IX_DotKhamSteps_UserId",
                table: "DotKhamSteps");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "DotKhamSteps");

            migrationBuilder.AddColumn<bool>(
                name: "IsDone",
                table: "DotKhamSteps",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDone",
                table: "DotKhamSteps");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "DotKhamSteps",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamSteps_UserId",
                table: "DotKhamSteps",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamSteps_AspNetUsers_UserId",
                table: "DotKhamSteps",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
