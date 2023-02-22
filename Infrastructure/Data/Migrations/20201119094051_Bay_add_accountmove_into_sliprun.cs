using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_add_accountmove_into_sliprun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdvanceMoney",
                table: "HrPayslips");

            migrationBuilder.AddColumn<Guid>(
                name: "MoveId",
                table: "HrPayslipRuns",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HrPayslipRuns_MoveId",
                table: "HrPayslipRuns",
                column: "MoveId");

            migrationBuilder.AddForeignKey(
                name: "FK_HrPayslipRuns_AccountMoves_MoveId",
                table: "HrPayslipRuns",
                column: "MoveId",
                principalTable: "AccountMoves",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HrPayslipRuns_AccountMoves_MoveId",
                table: "HrPayslipRuns");

            migrationBuilder.DropIndex(
                name: "IX_HrPayslipRuns_MoveId",
                table: "HrPayslipRuns");

            migrationBuilder.DropColumn(
                name: "MoveId",
                table: "HrPayslipRuns");

            migrationBuilder.AddColumn<decimal>(
                name: "AdvanceMoney",
                table: "HrPayslips",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
