using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddEmployeeToaThuoc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "ToaThuocs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuocs_EmployeeId",
                table: "ToaThuocs",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ToaThuocs_Employees_EmployeeId",
                table: "ToaThuocs",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToaThuocs_Employees_EmployeeId",
                table: "ToaThuocs");

            migrationBuilder.DropIndex(
                name: "IX_ToaThuocs_EmployeeId",
                table: "ToaThuocs");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "ToaThuocs");
        }
    }
}
