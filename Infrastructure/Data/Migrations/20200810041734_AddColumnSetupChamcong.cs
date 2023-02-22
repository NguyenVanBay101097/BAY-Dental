using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddColumnSetupChamcong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HalfStandardWorkFrom",
                table: "setupChamcongs");

            migrationBuilder.DropColumn(
                name: "HalfStandardWorkTo",
                table: "setupChamcongs");

            migrationBuilder.DropColumn(
                name: "OneStandardWorkFrom",
                table: "setupChamcongs");

            migrationBuilder.DropColumn(
                name: "OneStandardWorkTo",
                table: "setupChamcongs");

            migrationBuilder.DropColumn(
                name: "StandardWorkHour",
                table: "setupChamcongs");

            migrationBuilder.AddColumn<decimal>(
                name: "DifferenceTime",
                table: "setupChamcongs",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HalfStandardWorkHour",
                table: "setupChamcongs",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OneStandardWorkHour",
                table: "setupChamcongs",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DifferenceTime",
                table: "setupChamcongs");

            migrationBuilder.DropColumn(
                name: "HalfStandardWorkHour",
                table: "setupChamcongs");

            migrationBuilder.DropColumn(
                name: "OneStandardWorkHour",
                table: "setupChamcongs");

            migrationBuilder.AddColumn<decimal>(
                name: "HalfStandardWorkFrom",
                table: "setupChamcongs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HalfStandardWorkTo",
                table: "setupChamcongs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OneStandardWorkFrom",
                table: "setupChamcongs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OneStandardWorkTo",
                table: "setupChamcongs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "StandardWorkHour",
                table: "setupChamcongs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
