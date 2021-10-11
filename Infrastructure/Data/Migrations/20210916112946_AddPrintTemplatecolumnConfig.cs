using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddPrintTemplatecolumnConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "PrintTemplates",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PrintTemplateId",
                table: "PrintTemplateConfigs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrintTemplateConfigs_PrintTemplateId",
                table: "PrintTemplateConfigs",
                column: "PrintTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrintTemplateConfigs_PrintTemplates_PrintTemplateId",
                table: "PrintTemplateConfigs",
                column: "PrintTemplateId",
                principalTable: "PrintTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrintTemplateConfigs_PrintTemplates_PrintTemplateId",
                table: "PrintTemplateConfigs");

            migrationBuilder.DropIndex(
                name: "IX_PrintTemplateConfigs_PrintTemplateId",
                table: "PrintTemplateConfigs");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "PrintTemplates");

            migrationBuilder.DropColumn(
                name: "PrintTemplateId",
                table: "PrintTemplateConfigs");
        }
    }
}
