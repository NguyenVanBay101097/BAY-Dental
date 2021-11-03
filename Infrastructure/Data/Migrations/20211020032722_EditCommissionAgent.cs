using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class EditCommissionAgent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "PartnerId",
                table: "Agents",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "AccountHolder",
                table: "Agents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "Agents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankBranch",
                table: "Agents",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BankId",
                table: "Agents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Classify",
                table: "Agents",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "Agents",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "Agents",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agents_BankId",
                table: "Agents",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_Agents_CustomerId",
                table: "Agents",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Agents_EmployeeId",
                table: "Agents",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agents_ResBanks_BankId",
                table: "Agents",
                column: "BankId",
                principalTable: "ResBanks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Agents_Partners_CustomerId",
                table: "Agents",
                column: "CustomerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Agents_Employees_EmployeeId",
                table: "Agents",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agents_ResBanks_BankId",
                table: "Agents");

            migrationBuilder.DropForeignKey(
                name: "FK_Agents_Partners_CustomerId",
                table: "Agents");

            migrationBuilder.DropForeignKey(
                name: "FK_Agents_Employees_EmployeeId",
                table: "Agents");

            migrationBuilder.DropIndex(
                name: "IX_Agents_BankId",
                table: "Agents");

            migrationBuilder.DropIndex(
                name: "IX_Agents_CustomerId",
                table: "Agents");

            migrationBuilder.DropIndex(
                name: "IX_Agents_EmployeeId",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "AccountHolder",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "BankBranch",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "Classify",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Agents");

            migrationBuilder.AlterColumn<Guid>(
                name: "PartnerId",
                table: "Agents",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
