using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F103 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResConfigSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false),
                    GroupDiscountPerSOLine = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResConfigSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResConfigSettings_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResConfigSettings_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResConfigSettings_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResGroupImpliedRels",
                columns: table => new
                {
                    GId = table.Column<Guid>(nullable: false),
                    HId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResGroupImpliedRels", x => new { x.GId, x.HId });
                    table.ForeignKey(
                        name: "FK_ResGroupImpliedRels_ResGroups_GId",
                        column: x => x.GId,
                        principalTable: "ResGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResGroupImpliedRels_ResGroups_HId",
                        column: x => x.HId,
                        principalTable: "ResGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResConfigSettings_CompanyId",
                table: "ResConfigSettings",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ResConfigSettings_CreatedById",
                table: "ResConfigSettings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ResConfigSettings_WriteById",
                table: "ResConfigSettings",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ResGroupImpliedRels_HId",
                table: "ResGroupImpliedRels",
                column: "HId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResConfigSettings");

            migrationBuilder.DropTable(
                name: "ResGroupImpliedRels");
        }
    }
}
