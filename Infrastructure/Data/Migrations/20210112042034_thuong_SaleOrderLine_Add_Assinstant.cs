using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_SaleOrderLine_Add_Assinstant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssistantId",
                table: "SaleOrderLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_AssistantId",
                table: "SaleOrderLines",
                column: "AssistantId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderLines_Employees_AssistantId",
                table: "SaleOrderLines",
                column: "AssistantId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderLines_Employees_AssistantId",
                table: "SaleOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderLines_AssistantId",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "AssistantId",
                table: "SaleOrderLines");
        }
    }
}
