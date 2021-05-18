using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateSMS_8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SmsCampaignId",
                table: "SmsMessageDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessageDetails_SmsCampaignId",
                table: "SmsMessageDetails",
                column: "SmsCampaignId");

            migrationBuilder.AddForeignKey(
                name: "FK_SmsMessageDetails_SmsCampaign_SmsCampaignId",
                table: "SmsMessageDetails",
                column: "SmsCampaignId",
                principalTable: "SmsCampaign",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SmsMessageDetails_SmsCampaign_SmsCampaignId",
                table: "SmsMessageDetails");

            migrationBuilder.DropIndex(
                name: "IX_SmsMessageDetails_SmsCampaignId",
                table: "SmsMessageDetails");

            migrationBuilder.DropColumn(
                name: "SmsCampaignId",
                table: "SmsMessageDetails");
        }
    }
}
