using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Routings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Routings_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Routings_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Routings_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoutingLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    RoutingId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Sequence = table.Column<int>(nullable: false),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutingLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoutingLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoutingLines_Routings_RoutingId",
                        column: x => x.RoutingId,
                        principalTable: "Routings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoutingLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoutingLines_CreatedById",
                table: "RoutingLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RoutingLines_RoutingId",
                table: "RoutingLines",
                column: "RoutingId");

            migrationBuilder.CreateIndex(
                name: "IX_RoutingLines_WriteById",
                table: "RoutingLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_Routings_CreatedById",
                table: "Routings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Routings_ProductId",
                table: "Routings",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Routings_WriteById",
                table: "Routings",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoutingLines");

            migrationBuilder.DropTable(
                name: "Routings");
        }
    }
}
