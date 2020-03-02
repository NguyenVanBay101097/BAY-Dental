using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class FacebookConfigTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FacebookConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    FbAccountName = table.Column<string>(nullable: true),
                    FbAccountUserId = table.Column<string>(nullable: true),
                    UserAccessToken = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacebookConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacebookConfigs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacebookConfigs_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FacebookConfigPages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    ConfigId = table.Column<Guid>(nullable: false),
                    PageName = table.Column<string>(nullable: true),
                    PageId = table.Column<string>(nullable: true),
                    PageAccessToken = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacebookConfigPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacebookConfigPages_FacebookConfigs_ConfigId",
                        column: x => x.ConfigId,
                        principalTable: "FacebookConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacebookConfigPages_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacebookConfigPages_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FacebookConfigPages_ConfigId",
                table: "FacebookConfigPages",
                column: "ConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookConfigPages_CreatedById",
                table: "FacebookConfigPages",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookConfigPages_WriteById",
                table: "FacebookConfigPages",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookConfigs_CreatedById",
                table: "FacebookConfigs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FacebookConfigs_WriteById",
                table: "FacebookConfigs",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacebookConfigPages");

            migrationBuilder.DropTable(
                name: "FacebookConfigs");
        }
    }
}
