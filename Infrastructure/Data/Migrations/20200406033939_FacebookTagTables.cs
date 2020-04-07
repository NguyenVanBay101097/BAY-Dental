using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class FacebookTagTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FacebookTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacebookTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacebookTags_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacebookTags_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FacebookUserProfileTagRels",
                columns: table => new
                {
                    UserProfileId = table.Column<Guid>(nullable: false),
                    TagId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacebookUserProfileTagRels", x => new { x.UserProfileId, x.TagId });
                    table.ForeignKey(
                        name: "FK_FacebookUserProfileTagRels_FacebookTags_TagId",
                        column: x => x.TagId,
                        principalTable: "FacebookTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacebookUserProfileTagRels_FacebookUserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "FacebookUserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FacebookTags_CreatedById",
                table: "FacebookTags",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookTags_WriteById",
                table: "FacebookTags",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookUserProfileTagRels_TagId",
                table: "FacebookUserProfileTagRels",
                column: "TagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacebookUserProfileTagRels");

            migrationBuilder.DropTable(
                name: "FacebookTags");
        }
    }
}
