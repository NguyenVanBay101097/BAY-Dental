using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F22 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompleteName",
                table: "ProductCategories",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "ProductCategories",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentLeft",
                table: "ProductCategories",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentRight",
                table: "ProductCategories",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "ProductCategories",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ParentId",
                table: "ProductCategories",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_ProductCategories_ParentId",
                table: "ProductCategories",
                column: "ParentId",
                principalTable: "ProductCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_ProductCategories_ParentId",
                table: "ProductCategories");

            migrationBuilder.DropIndex(
                name: "IX_ProductCategories_ParentId",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "CompleteName",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "ParentLeft",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "ParentRight",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "ProductCategories");
        }
    }
}
