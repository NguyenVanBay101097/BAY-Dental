using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class FacebookConfigUpdate1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FacebookPageUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Psid = table.Column<string>(nullable: true),
                    ConfigPageId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacebookPageUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacebookPageUsers_FacebookConfigPages_ConfigPageId",
                        column: x => x.ConfigPageId,
                        principalTable: "FacebookConfigPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacebookPageUsers_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacebookPageUsers_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PartnerFacebookPageUserRels",
                columns: table => new
                {
                    PartnerId = table.Column<Guid>(nullable: false),
                    FbPageUserId = table.Column<Guid>(nullable: false),
                    Psid = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerFacebookPageUserRels", x => new { x.PartnerId, x.FbPageUserId });
                    table.ForeignKey(
                        name: "FK_PartnerFacebookPageUserRels_FacebookPageUsers_FbPageUserId",
                        column: x => x.FbPageUserId,
                        principalTable: "FacebookPageUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartnerFacebookPageUserRels_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FacebookPageUsers_ConfigPageId",
                table: "FacebookPageUsers",
                column: "ConfigPageId");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookPageUsers_CreatedById",
                table: "FacebookPageUsers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookPageUsers_WriteById",
                table: "FacebookPageUsers",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerFacebookPageUserRels_FbPageUserId",
                table: "PartnerFacebookPageUserRels",
                column: "FbPageUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartnerFacebookPageUserRels");

            migrationBuilder.DropTable(
                name: "FacebookPageUsers");
        }
    }
}
