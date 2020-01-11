using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F116 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_SaleCouponPrograms_CodePromoProgramId",
                table: "SaleOrders");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrders_SaleCouponPrograms_CodePromoProgramId",
                table: "SaleOrders",
                column: "CodePromoProgramId",
                principalTable: "SaleCouponPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrders_SaleCouponPrograms_CodePromoProgramId",
                table: "SaleOrders");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrders_SaleCouponPrograms_CodePromoProgramId",
                table: "SaleOrders",
                column: "CodePromoProgramId",
                principalTable: "SaleCouponPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
