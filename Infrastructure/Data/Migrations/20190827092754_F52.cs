using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F52 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeBSId",
                table: "DotKhams",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeKHId",
                table: "DotKhams",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_EmployeeBSId",
                table: "DotKhams",
                column: "EmployeeBSId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_EmployeeKHId",
                table: "DotKhams",
                column: "EmployeeKHId");

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_Employees_EmployeeBSId",
                table: "DotKhams",
                column: "EmployeeBSId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_Employees_EmployeeKHId",
                table: "DotKhams",
                column: "EmployeeKHId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_Employees_EmployeeBSId",
                table: "DotKhams");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_Employees_EmployeeKHId",
                table: "DotKhams");

            migrationBuilder.DropIndex(
                name: "IX_DotKhams_EmployeeBSId",
                table: "DotKhams");

            migrationBuilder.DropIndex(
                name: "IX_DotKhams_EmployeeKHId",
                table: "DotKhams");

            migrationBuilder.DropColumn(
                name: "EmployeeBSId",
                table: "DotKhams");

            migrationBuilder.DropColumn(
                name: "EmployeeKHId",
                table: "DotKhams");
        }
    }
}
