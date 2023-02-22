using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddMarketingTraceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketingTraces",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    ActivityId = table.Column<Guid>(nullable: false),
                    Sent = table.Column<DateTime>(nullable: true),
                    Exception = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketingTraces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketingTraces_MarketingCampaignActivities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "MarketingCampaignActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketingTraces_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarketingTraces_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketingTraces_ActivityId",
                table: "MarketingTraces",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingTraces_CreatedById",
                table: "MarketingTraces",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingTraces_WriteById",
                table: "MarketingTraces",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketingTraces");
        }
    }
}
