using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class addjounalType_into_revenueReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JournalTypes",
                table: "AccountFinancialRevenueReportAccountAccountTypeRels",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JournalTypes",
                table: "AccountFinancialRevenueReportAccountAccountRels",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JournalTypes",
                table: "AccountFinancialRevenueReportAccountAccountTypeRels");

            migrationBuilder.DropColumn(
                name: "JournalTypes",
                table: "AccountFinancialRevenueReportAccountAccountRels");
        }
    }
}
