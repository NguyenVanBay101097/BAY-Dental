using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddFacebookPageColumnToCampaign : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FacebookPageId",
                table: "MarketingCampaigns",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketingCampaigns_FacebookPageId",
                table: "MarketingCampaigns",
                column: "FacebookPageId");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketingCampaigns_FacebookPages_FacebookPageId",
                table: "MarketingCampaigns",
                column: "FacebookPageId",
                principalTable: "FacebookPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketingCampaigns_FacebookPages_FacebookPageId",
                table: "MarketingCampaigns");

            migrationBuilder.DropIndex(
                name: "IX_MarketingCampaigns_FacebookPageId",
                table: "MarketingCampaigns");

            migrationBuilder.DropColumn(
                name: "FacebookPageId",
                table: "MarketingCampaigns");
        }
    }
}
