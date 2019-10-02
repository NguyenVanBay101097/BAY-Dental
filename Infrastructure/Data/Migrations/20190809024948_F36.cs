using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F36 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateFinished",
                table: "DotKhamLines",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateStart",
                table: "DotKhamLines",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateFinished",
                table: "DotKhamLineOperations",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateStart",
                table: "DotKhamLineOperations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateFinished",
                table: "DotKhamLines");

            migrationBuilder.DropColumn(
                name: "DateStart",
                table: "DotKhamLines");

            migrationBuilder.DropColumn(
                name: "DateFinished",
                table: "DotKhamLineOperations");

            migrationBuilder.DropColumn(
                name: "DateStart",
                table: "DotKhamLineOperations");
        }
    }
}
