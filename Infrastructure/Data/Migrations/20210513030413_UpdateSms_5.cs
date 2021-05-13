using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateSms_5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "SmsMessages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "SmsConfigs",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SmsCampaignId",
                table: "SmsConfigs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SmsConfigs_SmsCampaignId",
                table: "SmsConfigs",
                column: "SmsCampaignId");

            migrationBuilder.AddForeignKey(
                name: "FK_SmsConfigs_SmsCampaign_SmsCampaignId",
                table: "SmsConfigs",
                column: "SmsCampaignId",
                principalTable: "SmsCampaign",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SmsConfigs_SmsCampaign_SmsCampaignId",
                table: "SmsConfigs");

            migrationBuilder.DropIndex(
                name: "IX_SmsConfigs_SmsCampaignId",
                table: "SmsConfigs");

            migrationBuilder.DropColumn(
                name: "Body",
                table: "SmsMessages");

            migrationBuilder.DropColumn(
                name: "Body",
                table: "SmsConfigs");

            migrationBuilder.DropColumn(
                name: "SmsCampaignId",
                table: "SmsConfigs");
        }
    }
}
