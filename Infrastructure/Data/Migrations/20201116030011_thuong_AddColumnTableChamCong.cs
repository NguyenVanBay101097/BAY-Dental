using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_AddColumnTableChamCong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OverTime",
                table: "ChamCongs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "OverTimeHour",
                table: "ChamCongs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ChamCongs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OverTime",
                table: "ChamCongs");

            migrationBuilder.DropColumn(
                name: "OverTimeHour",
                table: "ChamCongs");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ChamCongs");
        }
    }
}
