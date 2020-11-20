using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_addinto_payslip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ActualLeavePerMonth",
                table: "HrPayslips",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LeavePerMonthUnpaid",
                table: "HrPayslips",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualLeavePerMonth",
                table: "HrPayslips");

            migrationBuilder.DropColumn(
                name: "LeavePerMonthUnpaid",
                table: "HrPayslips");
        }
    }
}
