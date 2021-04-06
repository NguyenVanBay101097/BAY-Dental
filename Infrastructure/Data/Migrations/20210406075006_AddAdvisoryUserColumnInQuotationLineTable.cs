using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddAdvisoryUserColumnInQuotationLineTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationLines_Employees_AssistantId",
                table: "QuotationLines");

            migrationBuilder.DropForeignKey(
                name: "FK_QuotationLines_Employees_DoctorId",
                table: "QuotationLines");

            migrationBuilder.DropIndex(
                name: "IX_QuotationLines_AssistantId",
                table: "QuotationLines");

            migrationBuilder.DropIndex(
                name: "IX_QuotationLines_DoctorId",
                table: "QuotationLines");

            migrationBuilder.DropColumn(
                name: "AssistantId",
                table: "QuotationLines");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "QuotationLines");

            migrationBuilder.AddColumn<string>(
                name: "AdvisoryUserId",
                table: "QuotationLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_AdvisoryUserId",
                table: "QuotationLines",
                column: "AdvisoryUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationLines_AspNetUsers_AdvisoryUserId",
                table: "QuotationLines",
                column: "AdvisoryUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationLines_AspNetUsers_AdvisoryUserId",
                table: "QuotationLines");

            migrationBuilder.DropIndex(
                name: "IX_QuotationLines_AdvisoryUserId",
                table: "QuotationLines");

            migrationBuilder.DropColumn(
                name: "AdvisoryUserId",
                table: "QuotationLines");

            migrationBuilder.AddColumn<Guid>(
                name: "AssistantId",
                table: "QuotationLines",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DoctorId",
                table: "QuotationLines",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_AssistantId",
                table: "QuotationLines",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationLines_DoctorId",
                table: "QuotationLines",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationLines_Employees_AssistantId",
                table: "QuotationLines",
                column: "AssistantId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationLines_Employees_DoctorId",
                table: "QuotationLines",
                column: "DoctorId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
