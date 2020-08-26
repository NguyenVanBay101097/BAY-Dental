using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddColumnWorkEntryType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "WorkEntryTypes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "WorkEntryTypes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "WorkEntryTypes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "WorkEntryTypes");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "WorkEntryTypes");

            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "WorkEntryTypes");
        }
    }
}
