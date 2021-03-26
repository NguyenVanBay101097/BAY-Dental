using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_AddConfigPrintAndPrintPaperSize_Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PaperSizeId",
                table: "ResConfigSettings",
                nullable: true);

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
                    RightMargin = table.Column<int>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintPaperSizes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrintPaperSizes_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_ResConfigSettings_PaperSizeId",
                table: "ResConfigSettings",
                column: "PaperSizeId");

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
                name: "IX_PrintPaperSizes_CompanyId",
                table: "PrintPaperSizes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintPaperSizes_CreatedById",
                table: "PrintPaperSizes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PrintPaperSizes_WriteById",
                table: "PrintPaperSizes",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_ResConfigSettings_PrintPaperSizes_PaperSizeId",
                table: "ResConfigSettings",
                column: "PaperSizeId",
                principalTable: "PrintPaperSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResConfigSettings_PrintPaperSizes_PaperSizeId",
                table: "ResConfigSettings");

            migrationBuilder.DropTable(
                name: "ConfigPrints");

            migrationBuilder.DropTable(
                name: "PrintPaperSizes");

            migrationBuilder.DropIndex(
                name: "IX_ResConfigSettings_PaperSizeId",
                table: "ResConfigSettings");

            migrationBuilder.DropColumn(
                name: "PaperSizeId",
                table: "ResConfigSettings");
        }
    }
}
