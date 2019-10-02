using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F17 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KeToaNote",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "KeToaOK",
                table: "Products",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ToaThuocs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    PartnerId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    DotKhamId = table.Column<Guid>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToaThuocs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToaThuocs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToaThuocs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToaThuocs_DotKhams_DotKhamId",
                        column: x => x.DotKhamId,
                        principalTable: "DotKhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToaThuocs_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToaThuocs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToaThuocs_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ToaThuocLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    ToaThuocId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    Quantity = table.Column<decimal>(nullable: false),
                    Note = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToaThuocLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToaThuocLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToaThuocLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToaThuocLines_ToaThuocs_ToaThuocId",
                        column: x => x.ToaThuocId,
                        principalTable: "ToaThuocs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ToaThuocLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuocLines_CreatedById",
                table: "ToaThuocLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuocLines_ProductId",
                table: "ToaThuocLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuocLines_ToaThuocId",
                table: "ToaThuocLines",
                column: "ToaThuocId");

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuocLines_WriteById",
                table: "ToaThuocLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuocs_CompanyId",
                table: "ToaThuocs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuocs_CreatedById",
                table: "ToaThuocs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuocs_DotKhamId",
                table: "ToaThuocs",
                column: "DotKhamId");

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuocs_PartnerId",
                table: "ToaThuocs",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuocs_UserId",
                table: "ToaThuocs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuocs_WriteById",
                table: "ToaThuocs",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ToaThuocLines");

            migrationBuilder.DropTable(
                name: "ToaThuocs");

            migrationBuilder.DropColumn(
                name: "KeToaNote",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "KeToaOK",
                table: "Products");
        }
    }
}
