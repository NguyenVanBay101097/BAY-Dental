using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_addIsAccountingLoaiPhieu_ThuChi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAccounting",
                table: "PhieuThuChis",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAccounting",
                table: "LoaiThuChis",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccounting",
                table: "PhieuThuChis");

            migrationBuilder.DropColumn(
                name: "IsAccounting",
                table: "LoaiThuChis");
        }
    }
}
