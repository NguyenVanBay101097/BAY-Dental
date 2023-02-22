using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddTableSetupChamcong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "setupChamcongs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    StandardWorkHour = table.Column<decimal>(nullable: false),
                    OneStandardWorkFrom = table.Column<decimal>(nullable: false),
                    OneStandardWorkTo = table.Column<decimal>(nullable: false),
                    HalfStandardWorkFrom = table.Column<decimal>(nullable: false),
                    HalfStandardWorkTo = table.Column<decimal>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_setupChamcongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_setupChamcongs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_setupChamcongs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_setupChamcongs_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_setupChamcongs_CompanyId",
                table: "setupChamcongs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_setupChamcongs_CreatedById",
                table: "setupChamcongs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_setupChamcongs_WriteById",
                table: "setupChamcongs",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "setupChamcongs");
        }
    }
}
