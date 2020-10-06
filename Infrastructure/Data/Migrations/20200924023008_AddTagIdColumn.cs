using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddTagIdColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TagId",
                table: "TCareCampaigns",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TCareCampaigns_TagId",
                table: "TCareCampaigns",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_TCareCampaigns_PartnerCategories_TagId",
                table: "TCareCampaigns",
                column: "TagId",
                principalTable: "PartnerCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TCareCampaigns_PartnerCategories_TagId",
                table: "TCareCampaigns");

            migrationBuilder.DropIndex(
                name: "IX_TCareCampaigns_TagId",
                table: "TCareCampaigns");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "TCareCampaigns");
        }
    }
}
