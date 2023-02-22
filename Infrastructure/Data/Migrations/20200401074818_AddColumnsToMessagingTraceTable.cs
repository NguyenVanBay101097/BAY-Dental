using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddColumnsToMessagingTraceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Delivered",
                table: "FacebookMessagingTraces",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Opened",
                table: "FacebookMessagingTraces",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserProfileId",
                table: "FacebookMessagingTraces",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacebookMessagingTraces_UserProfileId",
                table: "FacebookMessagingTraces",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_FacebookMessagingTraces_FacebookUserProfiles_UserProfileId",
                table: "FacebookMessagingTraces",
                column: "UserProfileId",
                principalTable: "FacebookUserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacebookMessagingTraces_FacebookUserProfiles_UserProfileId",
                table: "FacebookMessagingTraces");

            migrationBuilder.DropIndex(
                name: "IX_FacebookMessagingTraces_UserProfileId",
                table: "FacebookMessagingTraces");

            migrationBuilder.DropColumn(
                name: "Delivered",
                table: "FacebookMessagingTraces");

            migrationBuilder.DropColumn(
                name: "Opened",
                table: "FacebookMessagingTraces");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "FacebookMessagingTraces");
        }
    }
}
