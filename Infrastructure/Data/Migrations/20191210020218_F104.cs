using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F104 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "ResGroups",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IrModuleCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ParentId = table.Column<Guid>(nullable: true),
                    Sequence = table.Column<int>(nullable: true),
                    Visible = table.Column<bool>(nullable: true),
                    Exclusive = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IrModuleCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IrModuleCategories_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IrModuleCategories_IrModuleCategories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "IrModuleCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IrModuleCategories_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResGroups_CategoryId",
                table: "ResGroups",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_IrModuleCategories_CreatedById",
                table: "IrModuleCategories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_IrModuleCategories_ParentId",
                table: "IrModuleCategories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_IrModuleCategories_WriteById",
                table: "IrModuleCategories",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_ResGroups_IrModuleCategories_CategoryId",
                table: "ResGroups",
                column: "CategoryId",
                principalTable: "IrModuleCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResGroups_IrModuleCategories_CategoryId",
                table: "ResGroups");

            migrationBuilder.DropTable(
                name: "IrModuleCategories");

            migrationBuilder.DropIndex(
                name: "IX_ResGroups_CategoryId",
                table: "ResGroups");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "ResGroups");
        }
    }
}
