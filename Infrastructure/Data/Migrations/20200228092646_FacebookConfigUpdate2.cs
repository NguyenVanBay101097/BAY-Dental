using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class FacebookConfigUpdate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartnerFacebookPageUserRels");

            migrationBuilder.AddColumn<Guid>(
                name: "PartnerId",
                table: "FacebookPageUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacebookPageUsers_PartnerId",
                table: "FacebookPageUsers",
                column: "PartnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_FacebookPageUsers_Partners_PartnerId",
                table: "FacebookPageUsers",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacebookPageUsers_Partners_PartnerId",
                table: "FacebookPageUsers");

            migrationBuilder.DropIndex(
                name: "IX_FacebookPageUsers_PartnerId",
                table: "FacebookPageUsers");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "FacebookPageUsers");

            migrationBuilder.CreateTable(
                name: "PartnerFacebookPageUserRels",
                columns: table => new
                {
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FbPageUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Psid = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "IX_PartnerFacebookPageUserRels_FbPageUserId",
                table: "PartnerFacebookPageUserRels",
                column: "FbPageUserId");
        }
    }
}
