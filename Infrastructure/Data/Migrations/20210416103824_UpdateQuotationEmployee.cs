using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateQuotationEmployee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_AspNetUsers_UserId",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_UserId",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Quotations");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "Quotations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_EmployeeId",
                table: "Quotations",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_Employees_EmployeeId",
                table: "Quotations",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_Employees_EmployeeId",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_EmployeeId",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Quotations");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Quotations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_UserId",
                table: "Quotations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_AspNetUsers_UserId",
                table: "Quotations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
