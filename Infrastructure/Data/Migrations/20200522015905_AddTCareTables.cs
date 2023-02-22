using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddTCareTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TCareCampaigns",
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
                    table.PrimaryKey("PK_TCareCampaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TCareCampaigns_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TCareCampaigns_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TCareRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    CampaignId = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TCareRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TCareRules_TCareCampaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "TCareCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TCareRules_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TCareRules_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TCareProperties",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    RuleId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: false),
                    ValueText = table.Column<string>(nullable: true),
                    ValueInteger = table.Column<int>(nullable: true),
                    ValueDateTime = table.Column<DateTime>(nullable: true),
                    ValueDecimal = table.Column<decimal>(nullable: true),
                    ValueDouble = table.Column<double>(nullable: true),
                    ValueReference = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TCareProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TCareProperties_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TCareProperties_TCareRules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "TCareRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TCareProperties_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TCareCampaigns_CreatedById",
                table: "TCareCampaigns",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TCareCampaigns_WriteById",
                table: "TCareCampaigns",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_TCareProperties_CreatedById",
                table: "TCareProperties",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TCareProperties_RuleId",
                table: "TCareProperties",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_TCareProperties_WriteById",
                table: "TCareProperties",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_TCareRules_CampaignId",
                table: "TCareRules",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_TCareRules_CreatedById",
                table: "TCareRules",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TCareRules_WriteById",
                table: "TCareRules",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TCareProperties");

            migrationBuilder.DropTable(
                name: "TCareRules");

            migrationBuilder.DropTable(
                name: "TCareCampaigns");
        }
    }
}
