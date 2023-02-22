using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddColumnSaleOrderLineIdinLaboLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SaleOrderLineId",
                table: "LaboOrderLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrderLines_SaleOrderLineId",
                table: "LaboOrderLines",
                column: "SaleOrderLineId");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrderLines_SaleOrderLines_SaleOrderLineId",
                table: "LaboOrderLines",
                column: "SaleOrderLineId",
                principalTable: "SaleOrderLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaboOrderLines_SaleOrderLines_SaleOrderLineId",
                table: "LaboOrderLines");

            migrationBuilder.DropIndex(
                name: "IX_LaboOrderLines_SaleOrderLineId",
                table: "LaboOrderLines");

            migrationBuilder.DropColumn(
                name: "SaleOrderLineId",
                table: "LaboOrderLines");
        }
    }
}
