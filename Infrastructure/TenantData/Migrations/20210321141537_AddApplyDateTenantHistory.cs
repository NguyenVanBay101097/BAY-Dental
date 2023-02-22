using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.TenantData.Migrations
{
    public partial class AddApplyDateTenantHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApplyDate",
                table: "TenantExtendHistories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplyDate",
                table: "TenantExtendHistories");
        }
    }
}
