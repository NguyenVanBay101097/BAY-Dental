using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Add_Tax_and_SocialInsurance_to_HrPayslip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxNSocialInsurance",
                table: "HrPayslips");

            migrationBuilder.AddColumn<decimal>(
                name: "SocialInsurance",
                table: "HrPayslips",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Tax",
                table: "HrPayslips",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SocialInsurance",
                table: "HrPayslips");

            migrationBuilder.DropColumn(
                name: "Tax",
                table: "HrPayslips");

            migrationBuilder.AddColumn<decimal>(
                name: "TaxNSocialInsurance",
                table: "HrPayslips",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
