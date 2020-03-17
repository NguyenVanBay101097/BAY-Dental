using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddFacebookPageColumnToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FacebookPageId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FacebookPageId",
                table: "AspNetUsers",
                column: "FacebookPageId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_FacebookPages_FacebookPageId",
                table: "AspNetUsers",
                column: "FacebookPageId",
                principalTable: "FacebookPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_FacebookPages_FacebookPageId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_FacebookPageId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FacebookPageId",
                table: "AspNetUsers");
        }
    }
}
