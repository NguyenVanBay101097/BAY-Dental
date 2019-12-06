using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F102 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleCoupons_SaleCouponPrograms_ProgramId",
                table: "SaleCoupons");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleCoupons_SaleCouponPrograms_ProgramId",
                table: "SaleCoupons",
                column: "ProgramId",
                principalTable: "SaleCouponPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleCoupons_SaleCouponPrograms_ProgramId",
                table: "SaleCoupons");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleCoupons_SaleCouponPrograms_ProgramId",
                table: "SaleCoupons",
                column: "ProgramId",
                principalTable: "SaleCouponPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
