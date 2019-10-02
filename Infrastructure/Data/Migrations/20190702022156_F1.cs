using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IRSequences",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    NumberNext = table.Column<int>(nullable: false),
                    Implementation = table.Column<string>(nullable: true),
                    Padding = table.Column<int>(nullable: false),
                    NumberIncrement = table.Column<int>(nullable: false),
                    Prefix = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    Suffix = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IRSequences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IRSequences_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IRSequences_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IRSequences_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IRSequences_CompanyId",
                table: "IRSequences",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_IRSequences_CreatedById",
                table: "IRSequences",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_IRSequences_WriteById",
                table: "IRSequences",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IRSequences");
        }
    }
}
