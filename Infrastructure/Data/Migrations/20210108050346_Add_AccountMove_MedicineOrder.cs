using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Add_AccountMove_MedicineOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MoveId",
                table: "MedicineOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrders_MoveId",
                table: "MedicineOrders",
                column: "MoveId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicineOrders_AccountMoves_MoveId",
                table: "MedicineOrders",
                column: "MoveId",
                principalTable: "AccountMoves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicineOrders_AccountMoves_MoveId",
                table: "MedicineOrders");

            migrationBuilder.DropIndex(
                name: "IX_MedicineOrders_MoveId",
                table: "MedicineOrders");

            migrationBuilder.DropColumn(
                name: "MoveId",
                table: "MedicineOrders");
        }
    }
}
