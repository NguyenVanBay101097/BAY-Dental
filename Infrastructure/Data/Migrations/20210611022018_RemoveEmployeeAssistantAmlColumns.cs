using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class RemoveEmployeeAssistantAmlColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountMoveLines_Employees_AssistantId",
                table: "AccountMoveLines");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountMoveLines_Employees_EmployeeId",
                table: "AccountMoveLines");

            migrationBuilder.DropIndex(
                name: "IX_AccountMoveLines_AssistantId",
                table: "AccountMoveLines");

            migrationBuilder.DropIndex(
                name: "IX_AccountMoveLines_EmployeeId",
                table: "AccountMoveLines");

            migrationBuilder.DropColumn(
                name: "AssistantId",
                table: "AccountMoveLines");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "AccountMoveLines");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssistantId",
                table: "AccountMoveLines",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "AccountMoveLines",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoveLines_AssistantId",
                table: "AccountMoveLines",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoveLines_EmployeeId",
                table: "AccountMoveLines",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_Employees_AssistantId",
                table: "AccountMoveLines",
                column: "AssistantId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_Employees_EmployeeId",
                table: "AccountMoveLines",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
