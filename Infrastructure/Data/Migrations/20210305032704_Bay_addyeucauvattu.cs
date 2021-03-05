using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_addyeucauvattu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductBoms",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: false),
                    MaterialProductId = table.Column<Guid>(nullable: true),
                    ProductUOMId = table.Column<Guid>(nullable: false),
                    Quantity = table.Column<decimal>(nullable: false),
                    Sequence = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductBoms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductBoms_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductBoms_Products_MaterialProductId",
                        column: x => x.MaterialProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductBoms_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductBoms_UoMs_ProductUOMId",
                        column: x => x.ProductUOMId,
                        principalTable: "UoMs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductBoms_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    EmployeeId = table.Column<Guid>(nullable: true),
                    PickingId = table.Column<Guid>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    SaleOrderId = table.Column<Guid>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductRequests_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRequests_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRequests_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRequests_StockPickings_PickingId",
                        column: x => x.PickingId,
                        principalTable: "StockPickings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRequests_SaleOrders_SaleOrderId",
                        column: x => x.SaleOrderId,
                        principalTable: "SaleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRequests_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRequests_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductRequestLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: true),
                    ProductUOMId = table.Column<Guid>(nullable: true),
                    RequestId = table.Column<Guid>(nullable: false),
                    SaleOrderLineId = table.Column<Guid>(nullable: true),
                    ProductQty = table.Column<decimal>(nullable: false),
                    Sequence = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRequestLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductRequestLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRequestLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRequestLines_UoMs_ProductUOMId",
                        column: x => x.ProductUOMId,
                        principalTable: "UoMs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRequestLines_ProductRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "ProductRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductRequestLines_SaleOrderLines_SaleOrderLineId",
                        column: x => x.SaleOrderLineId,
                        principalTable: "SaleOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRequestLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductBoms_CreatedById",
                table: "ProductBoms",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBoms_MaterialProductId",
                table: "ProductBoms",
                column: "MaterialProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBoms_ProductId",
                table: "ProductBoms",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBoms_ProductUOMId",
                table: "ProductBoms",
                column: "ProductUOMId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBoms_WriteById",
                table: "ProductBoms",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestLines_CreatedById",
                table: "ProductRequestLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestLines_ProductId",
                table: "ProductRequestLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestLines_ProductUOMId",
                table: "ProductRequestLines",
                column: "ProductUOMId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestLines_RequestId",
                table: "ProductRequestLines",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestLines_SaleOrderLineId",
                table: "ProductRequestLines",
                column: "SaleOrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestLines_WriteById",
                table: "ProductRequestLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequests_CompanyId",
                table: "ProductRequests",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequests_CreatedById",
                table: "ProductRequests",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequests_EmployeeId",
                table: "ProductRequests",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequests_PickingId",
                table: "ProductRequests",
                column: "PickingId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequests_SaleOrderId",
                table: "ProductRequests",
                column: "SaleOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequests_UserId",
                table: "ProductRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequests_WriteById",
                table: "ProductRequests",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductBoms");

            migrationBuilder.DropTable(
                name: "ProductRequestLines");

            migrationBuilder.DropTable(
                name: "ProductRequests");
        }
    }
}
