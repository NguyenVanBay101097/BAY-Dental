using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F_Edit_MarketingCampaignActivity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketingCampaignActivities_MarketingCampaignActivities_ParentId",
                table: "MarketingCampaignActivities");

            migrationBuilder.DropIndex(
                name: "IX_MarketingCampaignActivities_ParentId",
                table: "MarketingCampaignActivities");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
