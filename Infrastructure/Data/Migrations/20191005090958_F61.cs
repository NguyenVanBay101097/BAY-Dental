using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F61 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IrAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    DatasFname = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ResName = table.Column<string>(nullable: true),
                    ResField = table.Column<string>(nullable: true),
                    ResModel = table.Column<string>(nullable: true),
                    ResId = table.Column<Guid>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    Type = table.Column<string>(nullable: false),
                    Url = table.Column<string>(nullable: true),
                    DbDatas = table.Column<byte[]>(nullable: true),
                    MineType = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    FileSize = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IrAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IrAttachments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IrAttachments_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IrAttachments_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IrAttachments_CompanyId",
                table: "IrAttachments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_IrAttachments_CreatedById",
                table: "IrAttachments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_IrAttachments_WriteById",
                table: "IrAttachments",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IrAttachments");
        }
    }
}
