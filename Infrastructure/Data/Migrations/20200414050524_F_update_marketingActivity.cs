using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F_update_marketingActivity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateIndex(
                name: "IX_MarketingCampaignActivities_ParentId",
                table: "MarketingCampaignActivities",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketingCampaignActivities_MarketingCampaignActivities_ParentId",
                table: "MarketingCampaignActivities",
                column: "ParentId",
                principalTable: "MarketingCampaignActivities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketingCampaignActivities_MarketingCampaignActivities_ParentId",
                table: "MarketingCampaignActivities");

            migrationBuilder.DropIndex(
                name: "IX_MarketingCampaignActivities_ParentId",
                table: "MarketingCampaignActivities");

            migrationBuilder.AddColumn<Guid>(
                name: "MarketingCampaignActivityId",
                table: "MarketingCampaignActivities",
                type: "uniqueidentifier",
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
    }
}
