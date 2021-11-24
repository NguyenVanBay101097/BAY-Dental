using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddAgentColumnSaleOrderLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AgentId",
                table: "SaleOrderLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLines_AgentId",
                table: "SaleOrderLines",
                column: "AgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderLines_Agents_AgentId",
                table: "SaleOrderLines",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderLines_Agents_AgentId",
                table: "SaleOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderLines_AgentId",
                table: "SaleOrderLines");

            migrationBuilder.DropColumn(
                name: "AgentId",
                table: "SaleOrderLines");
        }
    }
}
