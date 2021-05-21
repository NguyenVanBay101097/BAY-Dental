using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddDeleteCascadeAccountMove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountMoveLines_AccountMoves_MoveId",
                table: "AccountMoveLines");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_AccountMoves_MoveId",
                table: "AccountMoveLines",
                column: "MoveId",
                principalTable: "AccountMoves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountMoveLines_AccountMoves_MoveId",
                table: "AccountMoveLines");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_AccountMoves_MoveId",
                table: "AccountMoveLines",
                column: "MoveId",
                principalTable: "AccountMoves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
