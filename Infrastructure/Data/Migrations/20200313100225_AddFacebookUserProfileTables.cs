using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddFacebookUserProfileTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FacebookUserProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Gender = table.Column<string>(nullable: true),
                    PSID = table.Column<string>(nullable: true),
                    FbPageId = table.Column<Guid>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacebookUserProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacebookUserProfiles_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacebookUserProfiles_FacebookPages_FbPageId",
                        column: x => x.FbPageId,
                        principalTable: "FacebookPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacebookUserProfiles_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacebookUserProfiles_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FacebookUserProfiles_CreatedById",
                table: "FacebookUserProfiles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookUserProfiles_FbPageId",
                table: "FacebookUserProfiles",
                column: "FbPageId");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookUserProfiles_PartnerId",
                table: "FacebookUserProfiles",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookUserProfiles_WriteById",
                table: "FacebookUserProfiles",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacebookUserProfiles");
        }
    }
}
