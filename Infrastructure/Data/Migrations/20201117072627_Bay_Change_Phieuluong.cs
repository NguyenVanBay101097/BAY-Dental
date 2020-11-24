using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_Change_Phieuluong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AdvanceMoney",
                table: "HrPayslips",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Allowance",
                table: "HrPayslips",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmercementMoney",
                table: "HrPayslips",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionSalary",
                table: "HrPayslips",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DaySalary",
                table: "HrPayslips",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HolidayAllowance",
                table: "HrPayslips",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NetSalary",
                table: "HrPayslips",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherAllowance",
                table: "HrPayslips",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OverTimeDay",
                table: "HrPayslips",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OverTimeDaySalary",
                table: "HrPayslips",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OverTimeHour",
                table: "HrPayslips",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OverTimeHourSalary",
                table: "HrPayslips",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RewardSalary",
                table: "HrPayslips",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalBasicSalary",
                table: "HrPayslips",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalSalary",
                table: "HrPayslips",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "HrPayslipRuns",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdvanceMoney",
                table: "HrPayslips");

            migrationBuilder.DropColumn(
                name: "Allowance",
                table: "HrPayslips");

            migrationBuilder.DropColumn(
                name: "AmercementMoney",
                table: "HrPayslips");

            migrationBuilder.DropColumn(
                name: "CommissionSalary",
                table: "HrPayslips");

            migrationBuilder.DropColumn(
                name: "DaySalary",
                table: "HrPayslips");

            migrationBuilder.DropColumn(
                name: "HolidayAllowance",
                table: "HrPayslips");

            migrationBuilder.DropColumn(
                name: "NetSalary",
                table: "HrPayslips");

            migrationBuilder.DropColumn(
                name: "OtherAllowance",
                table: "HrPayslips");

            migrationBuilder.DropColumn(
                name: "OverTimeDay",
                table: "HrPayslips");

            migrationBuilder.DropColumn(
                name: "OverTimeDaySalary",
                table: "HrPayslips");

            migrationBuilder.DropColumn(
                name: "OverTimeHour",
                table: "HrPayslips");

            migrationBuilder.DropColumn(
                name: "OverTimeHourSalary",
                table: "HrPayslips");

            migrationBuilder.DropColumn(
                name: "RewardSalary",
                table: "HrPayslips");

            migrationBuilder.DropColumn(
                name: "TotalBasicSalary",
                table: "HrPayslips");

            migrationBuilder.DropColumn(
                name: "TotalSalary",
                table: "HrPayslips");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "HrPayslipRuns");
        }
    }
}
