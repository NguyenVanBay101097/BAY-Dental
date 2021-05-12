using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateSms_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SmsAccountId",
                table: "SmsMessages",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateSend",
                table: "SmsConfigs",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SmsAccountId",
                table: "SmsConfigs",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SmsCampaignId",
                table: "SmsConfigs",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimeBeforSend",
                table: "SmsConfigs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "SmsConfigs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SmsMessages_SmsAccountId",
                table: "SmsMessages",
                column: "SmsAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsConfigs_SmsAccountId",
                table: "SmsConfigs",
                column: "SmsAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsConfigs_SmsCampaignId",
                table: "SmsConfigs",
                column: "SmsCampaignId");

            migrationBuilder.AddForeignKey(
                name: "FK_SmsConfigs_SmsAccounts_SmsAccountId",
                table: "SmsConfigs",
                column: "SmsAccountId",
                principalTable: "SmsAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SmsConfigs_SmsCampaign_SmsCampaignId",
                table: "SmsConfigs",
                column: "SmsCampaignId",
                principalTable: "SmsCampaign",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SmsMessages_SmsAccounts_SmsAccountId",
                table: "SmsMessages",
                column: "SmsAccountId",
                principalTable: "SmsAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SmsConfigs_SmsAccounts_SmsAccountId",
                table: "SmsConfigs");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsConfigs_SmsCampaign_SmsCampaignId",
                table: "SmsConfigs");

            migrationBuilder.DropForeignKey(
                name: "FK_SmsMessages_SmsAccounts_SmsAccountId",
                table: "SmsMessages");

            migrationBuilder.DropIndex(
                name: "IX_SmsMessages_SmsAccountId",
                table: "SmsMessages");

            migrationBuilder.DropIndex(
                name: "IX_SmsConfigs_SmsAccountId",
                table: "SmsConfigs");

            migrationBuilder.DropIndex(
                name: "IX_SmsConfigs_SmsCampaignId",
                table: "SmsConfigs");

            migrationBuilder.DropColumn(
                name: "SmsAccountId",
                table: "SmsMessages");

            migrationBuilder.DropColumn(
                name: "DateSend",
                table: "SmsConfigs");

            migrationBuilder.DropColumn(
                name: "SmsAccountId",
                table: "SmsConfigs");

            migrationBuilder.DropColumn(
                name: "SmsCampaignId",
                table: "SmsConfigs");

            migrationBuilder.DropColumn(
                name: "TimeBeforSend",
                table: "SmsConfigs");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "SmsConfigs");
        }
    }
}
