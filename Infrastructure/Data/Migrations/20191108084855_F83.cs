using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F83 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountInvoiceLines_PurchaseOrderLines_PurchaseLineId",
                table: "AccountInvoiceLines");

            migrationBuilder.AddColumn<decimal>(
                name: "Residual",
                table: "SaleOrders",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ResCompanyUsersRels",
                columns: table => new
                {
                    CompanyId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResCompanyUsersRels", x => new { x.CompanyId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ResCompanyUsersRels_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResCompanyUsersRels_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResCompanyUsersRels_UserId",
                table: "ResCompanyUsersRels",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoiceLines_PurchaseOrderLines_PurchaseLineId",
                table: "AccountInvoiceLines",
                column: "PurchaseLineId",
                principalTable: "PurchaseOrderLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountInvoiceLines_PurchaseOrderLines_PurchaseLineId",
                table: "AccountInvoiceLines");

            migrationBuilder.DropTable(
                name: "ResCompanyUsersRels");

            migrationBuilder.DropColumn(
                name: "Residual",
                table: "SaleOrders");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountInvoiceLines_PurchaseOrderLines_PurchaseLineId",
                table: "AccountInvoiceLines",
                column: "PurchaseLineId",
                principalTable: "PurchaseOrderLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
