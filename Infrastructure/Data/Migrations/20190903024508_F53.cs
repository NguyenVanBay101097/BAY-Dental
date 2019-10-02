using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F53 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DotKhamSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    DotKhamId = table.Column<Guid>(nullable: true),
                    InvoicesId = table.Column<Guid>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: true),
                    IsInclude = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DotKhamSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DotKhamSteps_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DotKhamSteps_DotKhams_DotKhamId",
                        column: x => x.DotKhamId,
                        principalTable: "DotKhams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DotKhamSteps_AccountInvoices_InvoicesId",
                        column: x => x.InvoicesId,
                        principalTable: "AccountInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DotKhamSteps_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DotKhamSteps_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DotKhamSteps_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamSteps_CreatedById",
                table: "DotKhamSteps",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamSteps_DotKhamId",
                table: "DotKhamSteps",
                column: "DotKhamId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamSteps_InvoicesId",
                table: "DotKhamSteps",
                column: "InvoicesId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamSteps_ProductId",
                table: "DotKhamSteps",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamSteps_UserId",
                table: "DotKhamSteps",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DotKhamSteps_WriteById",
                table: "DotKhamSteps",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DotKhamSteps");
        }
    }
}
