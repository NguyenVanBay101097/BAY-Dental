using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_AddColumnsRound : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RoundDays",
                table: "WorkEntryTypes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoundDaysType",
                table: "WorkEntryTypes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoundDays",
                table: "WorkEntryTypes");

            migrationBuilder.DropColumn(
                name: "RoundDaysType",
                table: "WorkEntryTypes");
        }
    }
}
