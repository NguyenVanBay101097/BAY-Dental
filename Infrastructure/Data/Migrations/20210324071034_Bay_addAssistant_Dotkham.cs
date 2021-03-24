using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_addAssistant_Dotkham : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssistantId",
                table: "DotKhams",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_AssistantId",
                table: "DotKhams",
                column: "AssistantId");

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_Employees_AssistantId",
                table: "DotKhams",
                column: "AssistantId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_Employees_AssistantId",
                table: "DotKhams");

            migrationBuilder.DropIndex(
                name: "IX_DotKhams_AssistantId",
                table: "DotKhams");

            migrationBuilder.DropColumn(
                name: "AssistantId",
                table: "DotKhams");
        }
    }
}
