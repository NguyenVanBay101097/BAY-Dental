using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_UpdateLaboOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaboOrders_Partners_CustomerId",
                table: "LaboOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_LaboOrders_DotKhams_DotKhamId",
                table: "LaboOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_LaboOrders_SaleOrders_SaleOrderId",
                table: "LaboOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_LaboOrders_AspNetUsers_UserId",
                table: "LaboOrders");

            migrationBuilder.DropIndex(
                name: "IX_LaboOrders_CustomerId",
                table: "LaboOrders");

            migrationBuilder.DropIndex(
                name: "IX_LaboOrders_DotKhamId",
                table: "LaboOrders");

            migrationBuilder.DropIndex(
                name: "IX_LaboOrders_SaleOrderId",
                table: "LaboOrders");

            migrationBuilder.DropIndex(
                name: "IX_LaboOrders_UserId",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "DotKhamId",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "PartnerRef",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "SaleOrderId",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "State",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "LaboOrders");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "LaboOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "LaboOrders",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceUnit",
                table: "LaboOrders",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "LaboOrders",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "LaboOrders",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "SaleOrderLineId",
                table: "LaboOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WarrantyCode",
                table: "LaboOrders",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "WarrantyPeriod",
                table: "LaboOrders",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LaboOrderToothRels",
                columns: table => new
                {
                    LaboOrderId = table.Column<Guid>(nullable: false),
                    ToothId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboOrderToothRels", x => new { x.ToothId, x.LaboOrderId });
                    table.ForeignKey(
                        name: "FK_LaboOrderToothRels_LaboOrders_LaboOrderId",
                        column: x => x.LaboOrderId,
                        principalTable: "LaboOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboOrderToothRels_Teeth_ToothId",
                        column: x => x.ToothId,
                        principalTable: "Teeth",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_ProductId",
                table: "LaboOrders",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_SaleOrderLineId",
                table: "LaboOrders",
                column: "SaleOrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrderToothRels_LaboOrderId",
                table: "LaboOrderToothRels",
                column: "LaboOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrders_Products_ProductId",
                table: "LaboOrders",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrders_SaleOrderLines_SaleOrderLineId",
                table: "LaboOrders",
                column: "SaleOrderLineId",
                principalTable: "SaleOrderLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaboOrders_Products_ProductId",
                table: "LaboOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_LaboOrders_SaleOrderLines_SaleOrderLineId",
                table: "LaboOrders");

            migrationBuilder.DropTable(
                name: "LaboOrderToothRels");

            migrationBuilder.DropIndex(
                name: "IX_LaboOrders_ProductId",
                table: "LaboOrders");

            migrationBuilder.DropIndex(
                name: "IX_LaboOrders_SaleOrderLineId",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "PriceUnit",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "SaleOrderLineId",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "WarrantyCode",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "WarrantyPeriod",
                table: "LaboOrders");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "LaboOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DotKhamId",
                table: "LaboOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartnerRef",
                table: "LaboOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SaleOrderId",
                table: "LaboOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "LaboOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "LaboOrders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_CustomerId",
                table: "LaboOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_DotKhamId",
                table: "LaboOrders",
                column: "DotKhamId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_SaleOrderId",
                table: "LaboOrders",
                column: "SaleOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_UserId",
                table: "LaboOrders",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrders_Partners_CustomerId",
                table: "LaboOrders",
                column: "CustomerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrders_DotKhams_DotKhamId",
                table: "LaboOrders",
                column: "DotKhamId",
                principalTable: "DotKhams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrders_SaleOrders_SaleOrderId",
                table: "LaboOrders",
                column: "SaleOrderId",
                principalTable: "SaleOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrders_AspNetUsers_UserId",
                table: "LaboOrders",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
