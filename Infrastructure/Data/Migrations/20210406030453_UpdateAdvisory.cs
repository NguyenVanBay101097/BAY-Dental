using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class UpdateAdvisory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advisory_Partners_CustomerId",
                table: "Advisory");

            migrationBuilder.DropForeignKey(
                name: "FK_Advisory_ToothCategories_ToothCategoryId",
                table: "Advisory");

            migrationBuilder.AlterColumn<Guid>(
                name: "ToothCategoryId",
                table: "Advisory",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerId",
                table: "Advisory",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Advisory_Partners_CustomerId",
                table: "Advisory",
                column: "CustomerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Advisory_ToothCategories_ToothCategoryId",
                table: "Advisory",
                column: "ToothCategoryId",
                principalTable: "ToothCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advisory_Partners_CustomerId",
                table: "Advisory");

            migrationBuilder.DropForeignKey(
                name: "FK_Advisory_ToothCategories_ToothCategoryId",
                table: "Advisory");

            migrationBuilder.AlterColumn<Guid>(
                name: "ToothCategoryId",
                table: "Advisory",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerId",
                table: "Advisory",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_Advisory_Partners_CustomerId",
                table: "Advisory",
                column: "CustomerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Advisory_ToothCategories_ToothCategoryId",
                table: "Advisory",
                column: "ToothCategoryId",
                principalTable: "ToothCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
