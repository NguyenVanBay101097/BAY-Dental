using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddReportHeaderFooterCompanyTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReportFooter",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReportHeader",
                table: "Companies",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportFooter",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "ReportHeader",
                table: "Companies");
        }
    }
}
