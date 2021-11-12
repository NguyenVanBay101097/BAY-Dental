using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddCardTypeRelInSaleCouponProgram : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaleCouponProgramCardTypeRels",
                columns: table => new
                {
                    ProgramId = table.Column<Guid>(nullable: false),
                    CardTypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleCouponProgramCardTypeRels", x => new { x.ProgramId, x.CardTypeId });
                    table.ForeignKey(
                        name: "FK_SaleCouponProgramCardTypeRels_CardTypes_CardTypeId",
                        column: x => x.CardTypeId,
                        principalTable: "CardTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleCouponProgramCardTypeRels_SaleCouponPrograms_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "SaleCouponPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleCouponProgramCardTypeRels_CardTypeId",
                table: "SaleCouponProgramCardTypeRels",
                column: "CardTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleCouponProgramCardTypeRels");
        }
    }
}
