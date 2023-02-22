using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Add_Column_AssistantUser_Table_DotKham : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssistantUserId",
                table: "DotKhams",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_AssistantUserId",
                table: "DotKhams",
                column: "AssistantUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_AspNetUsers_AssistantUserId",
                table: "DotKhams",
                column: "AssistantUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_AspNetUsers_AssistantUserId",
                table: "DotKhams");

            migrationBuilder.DropIndex(
                name: "IX_DotKhams_AssistantUserId",
                table: "DotKhams");

            migrationBuilder.DropColumn(
                name: "AssistantUserId",
                table: "DotKhams");
        }
    }
}
