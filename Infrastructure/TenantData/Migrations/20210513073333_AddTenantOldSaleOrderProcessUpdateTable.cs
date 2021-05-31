using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.TenantData.Migrations
{
    public partial class AddTenantOldSaleOrderProcessUpdateTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProcessUpdateOldSaleOrder",
                table: "Tenants");

            migrationBuilder.CreateTable(
                name: "TenantOldSaleOrderProcessUpdates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<Guid>(nullable: false),
                    State = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantOldSaleOrderProcessUpdates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantOldSaleOrderProcessUpdates_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantOldSaleOrderProcessUpdates_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantOldSaleOrderProcessUpdates_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantOldSaleOrderProcessUpdates_CreatedById",
                table: "TenantOldSaleOrderProcessUpdates",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TenantOldSaleOrderProcessUpdates_TenantId",
                table: "TenantOldSaleOrderProcessUpdates",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantOldSaleOrderProcessUpdates_WriteById",
                table: "TenantOldSaleOrderProcessUpdates",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantOldSaleOrderProcessUpdates");

            migrationBuilder.AddColumn<bool>(
                name: "IsProcessUpdateOldSaleOrder",
                table: "Tenants",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
