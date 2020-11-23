using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_ChangeEmployeee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Allowance",
                table: "Employees",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LeavePerMonth",
                table: "Employees",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OvertimeRate",
                table: "Employees",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RegularHour",
                table: "Employees",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RestDayRate",
                table: "Employees",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Allowance",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "LeavePerMonth",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "OvertimeRate",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "RegularHour",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "RestDayRate",
                table: "Employees");
        }
    }
}
