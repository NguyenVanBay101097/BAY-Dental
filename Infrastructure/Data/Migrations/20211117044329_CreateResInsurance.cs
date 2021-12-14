using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CreateResInsurance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GroupInsurance",
                table: "ResConfigSettings",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInsurance",
                table: "Partners",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ResInsurances",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Avatar = table.Column<string>(nullable: true),
                    Representative = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResInsurances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResInsurances_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResInsurances_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResInsurances_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResInsurances_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResInsurancePayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    OrderId = table.Column<Guid>(nullable: true),
                    ResInsuranceId = table.Column<Guid>(nullable: true),
                    SaleOrderPaymentId = table.Column<Guid>(nullable: true),
                    MoveId = table.Column<Guid>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResInsurancePayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResInsurancePayments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResInsurancePayments_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResInsurancePayments_AccountMoves_MoveId",
                        column: x => x.MoveId,
                        principalTable: "AccountMoves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResInsurancePayments_SaleOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "SaleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResInsurancePayments_ResInsurances_ResInsuranceId",
                        column: x => x.ResInsuranceId,
                        principalTable: "ResInsurances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResInsurancePayments_SaleOrderPayments_SaleOrderPaymentId",
                        column: x => x.SaleOrderPaymentId,
                        principalTable: "SaleOrderPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResInsurancePayments_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResInsurancePaymentLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    SaleOrderLineId = table.Column<Guid>(nullable: false),
                    ResInsurancePaymentId = table.Column<Guid>(nullable: false),
                    PayType = table.Column<string>(nullable: true),
                    Percent = table.Column<decimal>(nullable: true),
                    FixedAmount = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResInsurancePaymentLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResInsurancePaymentLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResInsurancePaymentLines_ResInsurancePayments_ResInsurancePaymentId",
                        column: x => x.ResInsurancePaymentId,
                        principalTable: "ResInsurancePayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResInsurancePaymentLines_SaleOrderLines_SaleOrderLineId",
                        column: x => x.SaleOrderLineId,
                        principalTable: "SaleOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResInsurancePaymentLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResInsurancePaymentLines_CreatedById",
                table: "ResInsurancePaymentLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ResInsurancePaymentLines_ResInsurancePaymentId",
                table: "ResInsurancePaymentLines",
                column: "ResInsurancePaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_ResInsurancePaymentLines_SaleOrderLineId",
                table: "ResInsurancePaymentLines",
                column: "SaleOrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_ResInsurancePaymentLines_WriteById",
                table: "ResInsurancePaymentLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ResInsurancePayments_CompanyId",
                table: "ResInsurancePayments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ResInsurancePayments_CreatedById",
                table: "ResInsurancePayments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ResInsurancePayments_MoveId",
                table: "ResInsurancePayments",
                column: "MoveId");

            migrationBuilder.CreateIndex(
                name: "IX_ResInsurancePayments_OrderId",
                table: "ResInsurancePayments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ResInsurancePayments_ResInsuranceId",
                table: "ResInsurancePayments",
                column: "ResInsuranceId");

            migrationBuilder.CreateIndex(
                name: "IX_ResInsurancePayments_SaleOrderPaymentId",
                table: "ResInsurancePayments",
                column: "SaleOrderPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_ResInsurancePayments_WriteById",
                table: "ResInsurancePayments",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ResInsurances_CompanyId",
                table: "ResInsurances",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ResInsurances_CreatedById",
                table: "ResInsurances",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ResInsurances_PartnerId",
                table: "ResInsurances",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ResInsurances_WriteById",
                table: "ResInsurances",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResInsurancePaymentLines");

            migrationBuilder.DropTable(
                name: "ResInsurancePayments");

            migrationBuilder.DropTable(
                name: "ResInsurances");

            migrationBuilder.DropColumn(
                name: "GroupInsurance",
                table: "ResConfigSettings");

            migrationBuilder.DropColumn(
                name: "IsInsurance",
                table: "Partners");
        }
    }
}
