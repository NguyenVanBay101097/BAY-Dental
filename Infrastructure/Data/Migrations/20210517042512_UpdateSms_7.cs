using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateSms_7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateEnd",
                table: "SmsCampaign",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateStart",
                table: "SmsCampaign",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LimitMessage",
                table: "SmsCampaign",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "SmsCampaign",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeDate",
                table: "SmsCampaign",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateEnd",
                table: "SmsCampaign");

            migrationBuilder.DropColumn(
                name: "DateStart",
                table: "SmsCampaign");

            migrationBuilder.DropColumn(
                name: "LimitMessage",
                table: "SmsCampaign");

            migrationBuilder.DropColumn(
                name: "State",
                table: "SmsCampaign");

            migrationBuilder.DropColumn(
                name: "TypeDate",
                table: "SmsCampaign");
        }
    }
}
