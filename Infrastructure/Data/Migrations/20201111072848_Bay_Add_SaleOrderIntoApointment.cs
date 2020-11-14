using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_Add_SaleOrderIntoApointment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SaleOrderId",
                table: "Appointments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_SaleOrderId",
                table: "Appointments",
                column: "SaleOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_SaleOrders_SaleOrderId",
                table: "Appointments",
                column: "SaleOrderId",
                principalTable: "SaleOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_SaleOrders_SaleOrderId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_SaleOrderId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "SaleOrderId",
                table: "Appointments");
        }
    }
}
