using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddAccountMoveInHrPayslip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountMoveId",
                table: "HrPayslips",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HrPayslips_AccountMoveId",
                table: "HrPayslips",
                column: "AccountMoveId");

            migrationBuilder.AddForeignKey(
                name: "FK_HrPayslips_AccountMoves_AccountMoveId",
                table: "HrPayslips",
                column: "AccountMoveId",
                principalTable: "AccountMoves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HrPayslips_AccountMoves_AccountMoveId",
                table: "HrPayslips");

            migrationBuilder.DropIndex(
                name: "IX_HrPayslips_AccountMoveId",
                table: "HrPayslips");

            migrationBuilder.DropColumn(
                name: "AccountMoveId",
                table: "HrPayslips");
        }
    }
}
