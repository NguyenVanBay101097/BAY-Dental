using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddTCareResConfigColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GroupTCare",
                table: "ResConfigSettings",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TCareRunAt",
                table: "ResConfigSettings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupTCare",
                table: "ResConfigSettings");

            migrationBuilder.DropColumn(
                name: "TCareRunAt",
                table: "ResConfigSettings");
        }
    }
}
