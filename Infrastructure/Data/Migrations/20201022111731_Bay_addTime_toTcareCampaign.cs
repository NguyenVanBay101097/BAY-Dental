using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_addTime_toTcareCampaign : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SheduleStart",
                table: "TCareCampaigns");

            migrationBuilder.AddColumn<decimal>(
                name: "SheduleStartNumber",
                table: "TCareCampaigns",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SheduleStartType",
                table: "TCareCampaigns",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SheduleStartNumber",
                table: "TCareCampaigns");

            migrationBuilder.DropColumn(
                name: "SheduleStartType",
                table: "TCareCampaigns");

            migrationBuilder.AddColumn<DateTime>(
                name: "SheduleStart",
                table: "TCareCampaigns",
                type: "datetime2",
                nullable: true);
        }
    }
}
