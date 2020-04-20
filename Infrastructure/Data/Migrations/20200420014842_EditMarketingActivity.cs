using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class EditMarketingActivity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "MarketingCampaignActivities",
                nullable: true);

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

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "MarketingCampaignActivities");
        }
    }
}
