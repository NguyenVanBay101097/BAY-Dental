using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddAccountMoveStateLaboOrderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountMoveId",
                table: "LaboOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "LaboOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_AccountMoveId",
                table: "LaboOrders",
                column: "AccountMoveId");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrders_AccountMoves_AccountMoveId",
                table: "LaboOrders",
                column: "AccountMoveId",
                principalTable: "AccountMoves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaboOrders_AccountMoves_AccountMoveId",
                table: "LaboOrders");

            migrationBuilder.DropIndex(
                name: "IX_LaboOrders_AccountMoveId",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "AccountMoveId",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "State",
                table: "LaboOrders");
        }
    }
}
