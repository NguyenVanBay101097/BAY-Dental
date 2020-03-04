using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class MarketingCampaignTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketingCampaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    DateStart = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketingCampaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketingCampaigns_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarketingCampaigns_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MarketingCampaignActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    CampaignId = table.Column<Guid>(nullable: false),
                    Condition = table.Column<string>(nullable: true),
                    ActivityType = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    IntervalType = table.Column<string>(nullable: true),
                    IntervalNumber = table.Column<int>(nullable: true),
                    Sequence = table.Column<int>(nullable: true),
                    TriggerType = table.Column<string>(nullable: true),
                    JobId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketingCampaignActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketingCampaignActivities_MarketingCampaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "MarketingCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketingCampaignActivities_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MarketingCampaignActivities_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketingCampaignActivities_CampaignId",
                table: "MarketingCampaignActivities",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingCampaignActivities_CreatedById",
                table: "MarketingCampaignActivities",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingCampaignActivities_WriteById",
                table: "MarketingCampaignActivities",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingCampaigns_CreatedById",
                table: "MarketingCampaigns",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingCampaigns_WriteById",
                table: "MarketingCampaigns",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketingCampaignActivities");

            migrationBuilder.DropTable(
                name: "MarketingCampaigns");
        }
    }
}
