using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class add_DoctorToSaleorder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DoctorId",
                table: "SaleOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrders_DoctorId",
                table: "SaleOrders",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrders_Employees_DoctorId",
                table: "SaleOrders",
                column: "DoctorId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_Employees_DoctorId",
                table: "SaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrders_DoctorId",
                table: "SaleOrders");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "SaleOrders");
        }
    }
}
