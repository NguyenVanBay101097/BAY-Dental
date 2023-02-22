using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateTCareMessagingTraceColumnsV1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TCareCampaigns_TCareScenarios_TCareScenarioId",
                table: "TCareCampaigns");

            migrationBuilder.DropColumn(
                name: "Read",
                table: "TCareMessingTraces");

            migrationBuilder.AddColumn<string>(
                name: "Error",
                table: "TCareMessingTraces",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Opened",
                table: "TCareMessingTraces",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TCareScenarioId",
                table: "TCareCampaigns",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_TCareCampaigns_TCareScenarios_TCareScenarioId",
                table: "TCareCampaigns",
                column: "TCareScenarioId",
                principalTable: "TCareScenarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TCareCampaigns_TCareScenarios_TCareScenarioId",
                table: "TCareCampaigns");

            migrationBuilder.DropColumn(
                name: "Error",
                table: "TCareMessingTraces");

            migrationBuilder.DropColumn(
                name: "Opened",
                table: "TCareMessingTraces");

            migrationBuilder.AddColumn<DateTime>(
                name: "Read",
                table: "TCareMessingTraces",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TCareScenarioId",
                table: "TCareCampaigns",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TCareCampaigns_TCareScenarios_TCareScenarioId",
                table: "TCareCampaigns",
                column: "TCareScenarioId",
                principalTable: "TCareScenarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
