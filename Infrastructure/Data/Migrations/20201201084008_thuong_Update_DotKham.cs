using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class thuong_Update_DotKham : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DotKhamLines_Routings_RoutingId",
                table: "DotKhamLines");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_Employees_AssistantId",
                table: "DotKhams");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_AspNetUsers_AssistantUserId",
                table: "DotKhams");

            migrationBuilder.DropIndex(
                name: "IX_DotKhams_AssistantId",
                table: "DotKhams");

            migrationBuilder.DropIndex(
                name: "IX_DotKhams_AssistantUserId",
                table: "DotKhams");

            migrationBuilder.DropIndex(
                name: "IX_DotKhamLines_RoutingId",
                table: "DotKhamLines");

            migrationBuilder.DropColumn(
                name: "AssistantId",
                table: "DotKhams");

            migrationBuilder.DropColumn(
                name: "AssistantUserId",
                table: "DotKhams");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "DotKhams");

            migrationBuilder.DropColumn(
                name: "DateFinished",
                table: "DotKhamLines");

            migrationBuilder.DropColumn(
                name: "DateStart",
                table: "DotKhamLines");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "DotKhamLines");

            migrationBuilder.DropColumn(
                name: "RoutingId",
                table: "DotKhamLines");

            migrationBuilder.AddColumn<Guid>(
                name: "DotKhamLineId",
                table: "SaleOrderLineToothRels",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "DotKhams",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "DotKhamLines",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameStep",
                table: "DotKhamLines",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "DotKhamLines",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SaleOrderLineId",
                table: "DotKhamLines",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ToothCategoryId",
                table: "DotKhamLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLineToothRels_DotKhamLineId",
                table: "SaleOrderLineToothRels",
                column: "DotKhamLineId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLines_SaleOrderLineId",
                table: "DotKhamLines",
                column: "SaleOrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLines_ToothCategoryId",
                table: "DotKhamLines",
                column: "ToothCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamLines_SaleOrderLines_SaleOrderLineId",
                table: "DotKhamLines",
                column: "SaleOrderLineId",
                principalTable: "SaleOrderLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamLines_ToothCategories_ToothCategoryId",
                table: "DotKhamLines",
                column: "ToothCategoryId",
                principalTable: "ToothCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleOrderLineToothRels_DotKhamLines_DotKhamLineId",
                table: "SaleOrderLineToothRels",
                column: "DotKhamLineId",
                principalTable: "DotKhamLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DotKhamLines_SaleOrderLines_SaleOrderLineId",
                table: "DotKhamLines");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhamLines_ToothCategories_ToothCategoryId",
                table: "DotKhamLines");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleOrderLineToothRels_DotKhamLines_DotKhamLineId",
                table: "SaleOrderLineToothRels");

            migrationBuilder.DropIndex(
                name: "IX_SaleOrderLineToothRels_DotKhamLineId",
                table: "SaleOrderLineToothRels");

            migrationBuilder.DropIndex(
                name: "IX_DotKhamLines_SaleOrderLineId",
                table: "DotKhamLines");

            migrationBuilder.DropIndex(
                name: "IX_DotKhamLines_ToothCategoryId",
                table: "DotKhamLines");

            migrationBuilder.DropColumn(
                name: "DotKhamLineId",
                table: "SaleOrderLineToothRels");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "DotKhams");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "DotKhamLines");

            migrationBuilder.DropColumn(
                name: "NameStep",
                table: "DotKhamLines");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "DotKhamLines");

            migrationBuilder.DropColumn(
                name: "SaleOrderLineId",
                table: "DotKhamLines");

            migrationBuilder.DropColumn(
                name: "ToothCategoryId",
                table: "DotKhamLines");

            migrationBuilder.AddColumn<Guid>(
                name: "AssistantId",
                table: "DotKhams",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssistantUserId",
                table: "DotKhams",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "DotKhams",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateFinished",
                table: "DotKhamLines",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateStart",
                table: "DotKhamLines",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "DotKhamLines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RoutingId",
                table: "DotKhamLines",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_AssistantId",
                table: "DotKhams",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhams_AssistantUserId",
                table: "DotKhams",
                column: "AssistantUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamLines_RoutingId",
                table: "DotKhamLines",
                column: "RoutingId");

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhamLines_Routings_RoutingId",
                table: "DotKhamLines",
                column: "RoutingId",
                principalTable: "Routings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_Employees_AssistantId",
                table: "DotKhams",
                column: "AssistantId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_AspNetUsers_AssistantUserId",
                table: "DotKhams",
                column: "AssistantUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
