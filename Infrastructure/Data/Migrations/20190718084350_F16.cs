using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F16 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "DotKhamLines",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "DotKhamLineOperations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "DotKhamLines");

            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "DotKhamLineOperations");
        }
    }
}
