using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateTCareTablesOptimizeV1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FacebookPageId",
                table: "TCareMessagings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "TCareMessagings",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledDate",
                table: "TCareMessages",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FacebookPageId",
                table: "TCareCampaigns",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessagings_FacebookPageId",
                table: "TCareMessagings",
                column: "FacebookPageId");

            migrationBuilder.CreateIndex(
                name: "IX_TCareCampaigns_FacebookPageId",
                table: "TCareCampaigns",
                column: "FacebookPageId");

            migrationBuilder.AddForeignKey(
                name: "FK_TCareCampaigns_FacebookPages_FacebookPageId",
                table: "TCareCampaigns",
                column: "FacebookPageId",
                principalTable: "FacebookPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TCareMessagings_FacebookPages_FacebookPageId",
                table: "TCareMessagings",
                column: "FacebookPageId",
                principalTable: "FacebookPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TCareCampaigns_FacebookPages_FacebookPageId",
                table: "TCareCampaigns");

            migrationBuilder.DropForeignKey(
                name: "FK_TCareMessagings_FacebookPages_FacebookPageId",
                table: "TCareMessagings");

            migrationBuilder.DropIndex(
                name: "IX_TCareMessagings_FacebookPageId",
                table: "TCareMessagings");

            migrationBuilder.DropIndex(
                name: "IX_TCareCampaigns_FacebookPageId",
                table: "TCareCampaigns");

            migrationBuilder.DropColumn(
                name: "FacebookPageId",
                table: "TCareMessagings");

            migrationBuilder.DropColumn(
                name: "State",
                table: "TCareMessagings");

            migrationBuilder.DropColumn(
                name: "ScheduledDate",
                table: "TCareMessages");

            migrationBuilder.DropColumn(
                name: "FacebookPageId",
                table: "TCareCampaigns");
        }
    }
}
