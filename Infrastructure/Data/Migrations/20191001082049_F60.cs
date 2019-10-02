using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F60 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "Partners",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "Partners",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Partners",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Partners",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Partners_EmployeeId",
                table: "Partners",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_Employees_EmployeeId",
                table: "Partners",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partners_Employees_EmployeeId",
                table: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_Partners_EmployeeId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Partners");
        }
    }
}
