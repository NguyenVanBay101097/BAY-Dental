using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_addCodeColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AmountCodeCompute",
                table: "HrSalaryRules",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AmountPercentageBase",
                table: "HrSalaryRules",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountCodeCompute",
                table: "HrSalaryRules");

            migrationBuilder.DropColumn(
                name: "AmountPercentageBase",
                table: "HrSalaryRules");
        }
    }
}
