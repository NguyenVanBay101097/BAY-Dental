using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F78 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ToothCategoryId",
                table: "LaboOrderLines",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LaboOrderLineToothRels",
                columns: table => new
                {
                    LaboLineId = table.Column<Guid>(nullable: false),
                    ToothId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboOrderLineToothRels", x => new { x.LaboLineId, x.ToothId });
                    table.ForeignKey(
                        name: "FK_LaboOrderLineToothRels_LaboOrderLines_LaboLineId",
                        column: x => x.LaboLineId,
                        principalTable: "LaboOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LaboOrderLineToothRels_Teeth_ToothId",
                        column: x => x.ToothId,
                        principalTable: "Teeth",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrderLines_ToothCategoryId",
                table: "LaboOrderLines",
                column: "ToothCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrderLineToothRels_ToothId",
                table: "LaboOrderLineToothRels",
                column: "ToothId");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrderLines_ToothCategories_ToothCategoryId",
                table: "LaboOrderLines",
                column: "ToothCategoryId",
                principalTable: "ToothCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaboOrderLines_ToothCategories_ToothCategoryId",
                table: "LaboOrderLines");

            migrationBuilder.DropTable(
                name: "LaboOrderLineToothRels");

            migrationBuilder.DropIndex(
                name: "IX_LaboOrderLines_ToothCategoryId",
                table: "LaboOrderLines");

            migrationBuilder.DropColumn(
                name: "ToothCategoryId",
                table: "LaboOrderLines");
        }
    }
}
