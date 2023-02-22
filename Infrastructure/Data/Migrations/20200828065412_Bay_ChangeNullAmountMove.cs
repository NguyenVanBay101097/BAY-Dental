using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_ChangeNullAmountMove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HrPayslips_AccountMoves_AccountMoveId",
                table: "HrPayslips");

            migrationBuilder.AddForeignKey(
                name: "FK_HrPayslips_AccountMoves_AccountMoveId",
                table: "HrPayslips",
                column: "AccountMoveId",
                principalTable: "AccountMoves",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HrPayslips_AccountMoves_AccountMoveId",
                table: "HrPayslips");

            migrationBuilder.AddForeignKey(
                name: "FK_HrPayslips_AccountMoves_AccountMoveId",
                table: "HrPayslips",
                column: "AccountMoveId",
                principalTable: "AccountMoves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
