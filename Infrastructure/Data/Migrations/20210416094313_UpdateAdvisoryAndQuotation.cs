using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateAdvisoryAndQuotation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advisory_AspNetUsers_UserId",
                table: "Advisory");

            migrationBuilder.DropForeignKey(
                name: "FK_QuotationLines_AspNetUsers_AdvisoryUserId",
                table: "QuotationLines");

            migrationBuilder.DropIndex(
                name: "IX_QuotationLines_AdvisoryUserId",
                table: "QuotationLines");

            migrationBuilder.DropIndex(
                name: "IX_Advisory_UserId",
                table: "Advisory");

            migrationBuilder.DropColumn(
                name: "AdvisoryUserId",
                table: "QuotationLines");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Advisory");

            migrationBuilder.AddColumn<Guid>(
                name: "AdvisoryEmployeeId",
                table: "QuotationLines",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "Advisory",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_AdvisoryEmployeeId",
                table: "QuotationLines",
                column: "AdvisoryEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Advisory_EmployeeId",
                table: "Advisory",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Advisory_Employees_EmployeeId",
                table: "Advisory",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationLines_Employees_AdvisoryEmployeeId",
                table: "QuotationLines",
                column: "AdvisoryEmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advisory_Employees_EmployeeId",
                table: "Advisory");

            migrationBuilder.DropForeignKey(
                name: "FK_QuotationLines_Employees_AdvisoryEmployeeId",
                table: "QuotationLines");

            migrationBuilder.DropIndex(
                name: "IX_QuotationLines_AdvisoryEmployeeId",
                table: "QuotationLines");

            migrationBuilder.DropIndex(
                name: "IX_Advisory_EmployeeId",
                table: "Advisory");

            migrationBuilder.DropColumn(
                name: "AdvisoryEmployeeId",
                table: "QuotationLines");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Advisory");

            migrationBuilder.AddColumn<string>(
                name: "AdvisoryUserId",
                table: "QuotationLines",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Advisory",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_AdvisoryUserId",
                table: "QuotationLines",
                column: "AdvisoryUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Advisory_UserId",
                table: "Advisory",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Advisory_AspNetUsers_UserId",
                table: "Advisory",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationLines_AspNetUsers_AdvisoryUserId",
                table: "QuotationLines",
                column: "AdvisoryUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
