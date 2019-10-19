using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F67 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IRRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Global = table.Column<bool>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    PermUnlink = table.Column<bool>(nullable: false),
                    PermWrite = table.Column<bool>(nullable: false),
                    PermRead = table.Column<bool>(nullable: false),
                    PermCreate = table.Column<bool>(nullable: false),
                    ModelId = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IRRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IRRules_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IRRules_IRModels_ModelId",
                        column: x => x.ModelId,
                        principalTable: "IRModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IRRules_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RuleGroupRels",
                columns: table => new
                {
                    RuleId = table.Column<Guid>(nullable: false),
                    GroupId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleGroupRels", x => new { x.RuleId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_RuleGroupRels_ResGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "ResGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RuleGroupRels_IRRules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "IRRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IRRules_CreatedById",
                table: "IRRules",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_IRRules_ModelId",
                table: "IRRules",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_IRRules_WriteById",
                table: "IRRules",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_RuleGroupRels_GroupId",
                table: "RuleGroupRels",
                column: "GroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RuleGroupRels");

            migrationBuilder.DropTable(
                name: "IRRules");
        }
    }
}
