using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddColToothCategoryToAdvisory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ToothCategoryId",
                table: "Advisory",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Advisory_ToothCategoryId",
                table: "Advisory",
                column: "ToothCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Advisory_ToothCategories_ToothCategoryId",
                table: "Advisory",
                column: "ToothCategoryId",
                principalTable: "ToothCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advisory_ToothCategories_ToothCategoryId",
                table: "Advisory");

            migrationBuilder.DropIndex(
                name: "IX_Advisory_ToothCategoryId",
                table: "Advisory");

            migrationBuilder.DropColumn(
                name: "ToothCategoryId",
                table: "Advisory");
        }
    }
}
