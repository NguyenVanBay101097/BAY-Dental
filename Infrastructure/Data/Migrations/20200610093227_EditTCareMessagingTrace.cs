using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class EditTCareMessagingTrace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ChannelSocialId",
                table: "TCareMessingTraces",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessingTraces_ChannelSocialId",
                table: "TCareMessingTraces",
                column: "ChannelSocialId");

            migrationBuilder.AddForeignKey(
                name: "FK_TCareMessingTraces_FacebookPages_ChannelSocialId",
                table: "TCareMessingTraces",
                column: "ChannelSocialId",
                principalTable: "FacebookPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TCareMessingTraces_FacebookPages_ChannelSocialId",
                table: "TCareMessingTraces");

            migrationBuilder.DropIndex(
                name: "IX_TCareMessingTraces_ChannelSocialId",
                table: "TCareMessingTraces");

            migrationBuilder.DropColumn(
                name: "ChannelSocialId",
                table: "TCareMessingTraces");
        }
    }
}
