using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class luan_addColumnIntoSaleCouponProgram : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Days",
                table: "SaleCouponPrograms",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotIncremental",
                table: "SaleCouponPrograms",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SaleCouponProgramProductCategoryRels",
                columns: table => new
                {
                    ProgramId = table.Column<Guid>(nullable: false),
                    ProductCategoryId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleCouponProgramProductCategoryRels", x => new { x.ProgramId, x.ProductCategoryId });
                    table.ForeignKey(
                        name: "FK_SaleCouponProgramProductCategoryRels_ProductCategories_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleCouponProgramProductCategoryRels_SaleCouponPrograms_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "SaleCouponPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleCouponProgramProductCategoryRels_ProductCategoryId",
                table: "SaleCouponProgramProductCategoryRels",
                column: "ProductCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleCouponProgramProductCategoryRels");

            migrationBuilder.DropColumn(
                name: "Days",
                table: "SaleCouponPrograms");

            migrationBuilder.DropColumn(
                name: "NotIncremental",
                table: "SaleCouponPrograms");
        }
    }
}
