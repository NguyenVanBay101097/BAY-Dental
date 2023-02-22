using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CreatePartnerTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TitleId",
                table: "Partners",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PartnerTitles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerTitles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartnerTitles_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartnerTitles_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Partners_TitleId",
                table: "Partners",
                column: "TitleId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerTitles_CreatedById",
                table: "PartnerTitles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerTitles_WriteById",
                table: "PartnerTitles",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_PartnerTitles_TitleId",
                table: "Partners",
                column: "TitleId",
                principalTable: "PartnerTitles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partners_PartnerTitles_TitleId",
                table: "Partners");

            migrationBuilder.DropTable(
                name: "PartnerTitles");

            migrationBuilder.DropIndex(
                name: "IX_Partners_TitleId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "TitleId",
                table: "Partners");
        }
    }
}
