using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddColumnCampaign : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecurringJobId",
                table: "TCareCampaigns",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SheduleStart",
                table: "TCareCampaigns",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "TCareCampaigns",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecurringJobId",
                table: "TCareCampaigns");

            migrationBuilder.DropColumn(
                name: "SheduleStart",
                table: "TCareCampaigns");

            migrationBuilder.DropColumn(
                name: "State",
                table: "TCareCampaigns");
        }
    }
}
