using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F68 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountMoveLines_AccountFullReconciles_FullReconcileId",
                table: "AccountMoveLines");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_AccountFullReconciles_FullReconcileId",
                table: "AccountMoveLines",
                column: "FullReconcileId",
                principalTable: "AccountFullReconciles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountMoveLines_AccountFullReconciles_FullReconcileId",
                table: "AccountMoveLines");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_AccountFullReconciles_FullReconcileId",
                table: "AccountMoveLines",
                column: "FullReconcileId",
                principalTable: "AccountFullReconciles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
