using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F_Edit_2_MarketingCampaignActivity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MarketingCampaignActivityId",
                table: "MarketingCampaignActivities",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketingCampaignActivities_MarketingCampaignActivityId",
                table: "MarketingCampaignActivities",
                column: "MarketingCampaignActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketingCampaignActivities_MarketingCampaignActivities_MarketingCampaignActivityId",
                table: "MarketingCampaignActivities",
                column: "MarketingCampaignActivityId",
                principalTable: "MarketingCampaignActivities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketingCampaignActivities_MarketingCampaignActivities_MarketingCampaignActivityId",
                table: "MarketingCampaignActivities");

            migrationBuilder.DropIndex(
                name: "IX_MarketingCampaignActivities_MarketingCampaignActivityId",
                table: "MarketingCampaignActivities");

            migrationBuilder.DropColumn(
                name: "MarketingCampaignActivityId",
                table: "MarketingCampaignActivities");
        }
    }
}
