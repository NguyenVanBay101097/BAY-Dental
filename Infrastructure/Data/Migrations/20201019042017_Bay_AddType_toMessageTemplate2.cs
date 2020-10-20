using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_AddType_toMessageTemplate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CouponProgramId",
                table: "TCareMessageTemplates",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TCareMessageTemplates_CouponProgramId",
                table: "TCareMessageTemplates",
                column: "CouponProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_TCareMessageTemplates_SaleCouponPrograms_CouponProgramId",
                table: "TCareMessageTemplates",
                column: "CouponProgramId",
                principalTable: "SaleCouponPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TCareMessageTemplates_SaleCouponPrograms_CouponProgramId",
                table: "TCareMessageTemplates");

            migrationBuilder.DropIndex(
                name: "IX_TCareMessageTemplates_CouponProgramId",
                table: "TCareMessageTemplates");

            migrationBuilder.DropColumn(
                name: "CouponProgramId",
                table: "TCareMessageTemplates");
        }
    }
}
