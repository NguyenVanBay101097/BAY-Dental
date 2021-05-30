using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_EditCouponProgram : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplyPartnerOn",
                table: "SaleCouponPrograms",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApplyMaxDiscount",
                table: "SaleCouponPrograms",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsApplyMinimumDiscount",
                table: "SaleCouponPrograms",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "SaleCouponProgramPartnerRels",
                columns: table => new
                {
                    ProgramId = table.Column<Guid>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleCouponProgramPartnerRels", x => new { x.ProgramId, x.PartnerId });
                    table.ForeignKey(
                        name: "FK_SaleCouponProgramPartnerRels_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleCouponProgramPartnerRels_SaleCouponPrograms_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "SaleCouponPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleCouponProgramPartnerRels_PartnerId",
                table: "SaleCouponProgramPartnerRels",
                column: "PartnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleCouponProgramPartnerRels");

            migrationBuilder.DropColumn(
                name: "ApplyPartnerOn",
                table: "SaleCouponPrograms");

            migrationBuilder.DropColumn(
                name: "IsApplyMaxDiscount",
                table: "SaleCouponPrograms");

            migrationBuilder.DropColumn(
                name: "IsApplyMinimumDiscount",
                table: "SaleCouponPrograms");
        }
    }
}
