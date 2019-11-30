using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F94 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActivatedDate",
                table: "CardCards",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "CardCards",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivatedDate",
                table: "CardCards");

            migrationBuilder.DropColumn(
                name: "State",
                table: "CardCards");
        }
    }
}
