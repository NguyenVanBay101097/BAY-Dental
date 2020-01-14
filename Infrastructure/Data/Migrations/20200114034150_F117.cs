using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F117 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountPartialReconciles_AccountFullReconciles_FullReconcileId",
                table: "AccountPartialReconciles");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountPartialReconciles_AccountFullReconciles_FullReconcileId",
                table: "AccountPartialReconciles",
                column: "FullReconcileId",
                principalTable: "AccountFullReconciles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountPartialReconciles_AccountFullReconciles_FullReconcileId",
                table: "AccountPartialReconciles");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountPartialReconciles_AccountFullReconciles_FullReconcileId",
                table: "AccountPartialReconciles",
                column: "FullReconcileId",
                principalTable: "AccountFullReconciles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
