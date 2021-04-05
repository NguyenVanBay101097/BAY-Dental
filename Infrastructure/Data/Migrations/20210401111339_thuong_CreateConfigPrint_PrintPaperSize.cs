using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_CreateConfigPrint_PrintPaperSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrintPaperSizes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PaperFormat = table.Column<string>(nullable: true),
                    TopMargin = table.Column<int>(nullable: false),
                    BottomMargin = table.Column<int>(nullable: false),
                    LeftMargin = table.Column<int>(nullable: false),
                    RightMargin = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintPaperSizes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrintPaperSizes_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrintPaperSizes_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConfigPrints",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    PaperSizeId = table.Column<Guid>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    IsInfoCompany = table.Column<bool>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigPrints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfigPrints_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConfigPrints_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConfigPrints_PrintPaperSizes_PaperSizeId",
                        column: x => x.PaperSizeId,
                        principalTable: "PrintPaperSizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConfigPrints_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfigPrints_CompanyId",
                table: "ConfigPrints",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigPrints_CreatedById",
                table: "ConfigPrints",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigPrints_PaperSizeId",
                table: "ConfigPrints",
                column: "PaperSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigPrints_WriteById",
                table: "ConfigPrints",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_PrintPaperSizes_CreatedById",
                table: "PrintPaperSizes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PrintPaperSizes_WriteById",
                table: "PrintPaperSizes",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfigPrints");

            migrationBuilder.DropTable(
                name: "PrintPaperSizes");
        }
    }
}
