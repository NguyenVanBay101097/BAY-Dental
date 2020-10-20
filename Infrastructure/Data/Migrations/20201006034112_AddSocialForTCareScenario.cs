using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddSocialForTCareScenario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TCareMessagings_FacebookPages_ChannelSocialId",
                table: "TCareMessagings");

            migrationBuilder.DropIndex(
                name: "IX_TCareMessagings_ChannelSocialId",
                table: "TCareMessagings");

            migrationBuilder.DropColumn(
                name: "ChannelSocialId",
                table: "TCareMessagings");

            migrationBuilder.DropColumn(
                name: "ChannelType",
                table: "TCareMessagings");

            migrationBuilder.AddColumn<Guid>(
                name: "ChannelSocialId",
                table: "TCareScenarios",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TCareScenarios_ChannelSocialId",
                table: "TCareScenarios",
                column: "ChannelSocialId");

            migrationBuilder.AddForeignKey(
                name: "FK_TCareScenarios_FacebookPages_ChannelSocialId",
                table: "TCareScenarios",
                column: "ChannelSocialId",
                principalTable: "FacebookPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TCareScenarios_FacebookPages_ChannelSocialId",
                table: "TCareScenarios");

            migrationBuilder.DropIndex(
                name: "IX_TCareScenarios_ChannelSocialId",
                table: "TCareScenarios");

            migrationBuilder.DropColumn(
                name: "ChannelSocialId",
                table: "TCareScenarios");

            migrationBuilder.AddColumn<Guid>(
                name: "ChannelSocialId",
                table: "TCareMessagings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChannelType",
                table: "TCareMessagings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessagings_ChannelSocialId",
                table: "TCareMessagings",
                column: "ChannelSocialId");

            migrationBuilder.AddForeignKey(
                name: "FK_TCareMessagings_FacebookPages_ChannelSocialId",
                table: "TCareMessagings",
                column: "ChannelSocialId",
                principalTable: "FacebookPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
