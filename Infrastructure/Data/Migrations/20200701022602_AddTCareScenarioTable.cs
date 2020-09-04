using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddTCareScenarioTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TCareScenarioId",
                table: "TCareCampaigns",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "TCareScenarios",
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
                    table.PrimaryKey("PK_TCareScenarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TCareScenarios_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TCareScenarios_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TCareCampaigns_TCareScenarioId",
                table: "TCareCampaigns",
                column: "TCareScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TCareScenarios_CreatedById",
                table: "TCareScenarios",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TCareScenarios_WriteById",
                table: "TCareScenarios",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_TCareCampaigns_TCareScenarios_TCareScenarioId",
                table: "TCareCampaigns",
                column: "TCareScenarioId",
                principalTable: "TCareScenarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TCareCampaigns_TCareScenarios_TCareScenarioId",
                table: "TCareCampaigns");

            migrationBuilder.DropTable(
                name: "TCareScenarios");

            migrationBuilder.DropIndex(
                name: "IX_TCareCampaigns_TCareScenarioId",
                table: "TCareCampaigns");

            migrationBuilder.DropColumn(
                name: "TCareScenarioId",
                table: "TCareCampaigns");
        }
    }
}
