using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class PartnerMapPSIdTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PartnerMapPSIDFacebookPages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: false),
                    PageId = table.Column<string>(nullable: true),
                    PSId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerMapPSIDFacebookPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartnerMapPSIDFacebookPages_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartnerMapPSIDFacebookPages_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartnerMapPSIDFacebookPages_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartnerMapPSIDFacebookPages_CreatedById",
                table: "PartnerMapPSIDFacebookPages",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerMapPSIDFacebookPages_PartnerId",
                table: "PartnerMapPSIDFacebookPages",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerMapPSIDFacebookPages_WriteById",
                table: "PartnerMapPSIDFacebookPages",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartnerMapPSIDFacebookPages");
        }
    }
}
