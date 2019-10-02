using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F39 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "PartnerCategories",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CompleteName",
                table: "PartnerCategories",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "PartnerCategories",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentLeft",
                table: "PartnerCategories",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentRight",
                table: "PartnerCategories",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartnerCategories_ParentId",
                table: "PartnerCategories",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerCategories_PartnerCategories_ParentId",
                table: "PartnerCategories",
                column: "ParentId",
                principalTable: "PartnerCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerCategories_PartnerCategories_ParentId",
                table: "PartnerCategories");

            migrationBuilder.DropIndex(
                name: "IX_PartnerCategories_ParentId",
                table: "PartnerCategories");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "PartnerCategories");

            migrationBuilder.DropColumn(
                name: "CompleteName",
                table: "PartnerCategories");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "PartnerCategories");

            migrationBuilder.DropColumn(
                name: "ParentLeft",
                table: "PartnerCategories");

            migrationBuilder.DropColumn(
                name: "ParentRight",
                table: "PartnerCategories");
        }
    }
}
