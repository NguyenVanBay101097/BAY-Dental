using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CreatePrintTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrintTemplateConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    PrintPaperSizeId = table.Column<Guid>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintTemplateConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrintTemplateConfigs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrintTemplateConfigs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrintTemplateConfigs_PrintPaperSizes_PrintPaperSizeId",
                        column: x => x.PrintPaperSizeId,
                        principalTable: "PrintPaperSizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrintTemplateConfigs_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrintTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrintTemplates_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrintTemplates_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrintTemplateConfigs_CompanyId",
                table: "PrintTemplateConfigs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintTemplateConfigs_CreatedById",
                table: "PrintTemplateConfigs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PrintTemplateConfigs_PrintPaperSizeId",
                table: "PrintTemplateConfigs",
                column: "PrintPaperSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintTemplateConfigs_WriteById",
                table: "PrintTemplateConfigs",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_PrintTemplates_CreatedById",
                table: "PrintTemplates",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PrintTemplates_WriteById",
                table: "PrintTemplates",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrintTemplateConfigs");

            migrationBuilder.DropTable(
                name: "PrintTemplates");
        }
    }
}
