using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CreateSaleOrderLineCommission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CommissionId",
                table: "Employees",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Commissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Commissions_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Commissions_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Commissions_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaleOrderLinePaymentRels",
                columns: table => new
                {
                    SaleOrderLineId = table.Column<Guid>(nullable: false),
                    PaymentId = table.Column<Guid>(nullable: false),
                    AmountPrepaid = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrderLinePaymentRels", x => new { x.PaymentId, x.SaleOrderLineId });
                    table.ForeignKey(
                        name: "FK_SaleOrderLinePaymentRels_AccountPayments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "AccountPayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderLinePaymentRels_SaleOrderLines_SaleOrderLineId",
                        column: x => x.SaleOrderLineId,
                        principalTable: "SaleOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommissionProductRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    AppliedOn = table.Column<string>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: true),
                    CategId = table.Column<Guid>(nullable: true),
                    PercentFixed = table.Column<decimal>(nullable: true),
                    CommissionId = table.Column<Guid>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionProductRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommissionProductRules_ProductCategories_CategId",
                        column: x => x.CategId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommissionProductRules_Commissions_CommissionId",
                        column: x => x.CommissionId,
                        principalTable: "Commissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommissionProductRules_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommissionProductRules_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommissionProductRules_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommissionProductRules_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaleOrderLinePartnerCommissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: true),
                    SaleOrderLineId = table.Column<Guid>(nullable: false),
                    CommissionId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrderLinePartnerCommissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleOrderLinePartnerCommissions_Commissions_CommissionId",
                        column: x => x.CommissionId,
                        principalTable: "Commissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderLinePartnerCommissions_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderLinePartnerCommissions_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderLinePartnerCommissions_SaleOrderLines_SaleOrderLineId",
                        column: x => x.SaleOrderLineId,
                        principalTable: "SaleOrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleOrderLinePartnerCommissions_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CommissionId",
                table: "Employees",
                column: "CommissionId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EmployeeId",
                table: "AspNetUsers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionProductRules_CategId",
                table: "CommissionProductRules",
                column: "CategId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionProductRules_CommissionId",
                table: "CommissionProductRules",
                column: "CommissionId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionProductRules_CompanyId",
                table: "CommissionProductRules",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionProductRules_CreatedById",
                table: "CommissionProductRules",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionProductRules_ProductId",
                table: "CommissionProductRules",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionProductRules_WriteById",
                table: "CommissionProductRules",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_CompanyId",
                table: "Commissions",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_CreatedById",
                table: "Commissions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_WriteById",
                table: "Commissions",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLinePartnerCommissions_CommissionId",
                table: "SaleOrderLinePartnerCommissions",
                column: "CommissionId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLinePartnerCommissions_CreatedById",
                table: "SaleOrderLinePartnerCommissions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLinePartnerCommissions_PartnerId",
                table: "SaleOrderLinePartnerCommissions",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLinePartnerCommissions_SaleOrderLineId",
                table: "SaleOrderLinePartnerCommissions",
                column: "SaleOrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLinePartnerCommissions_WriteById",
                table: "SaleOrderLinePartnerCommissions",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderLinePaymentRels_SaleOrderLineId",
                table: "SaleOrderLinePaymentRels",
                column: "SaleOrderLineId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Employees_EmployeeId",
                table: "AspNetUsers",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Commissions_CommissionId",
                table: "Employees",
                column: "CommissionId",
                principalTable: "Commissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Employees_EmployeeId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Commissions_CommissionId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "CommissionProductRules");

            migrationBuilder.DropTable(
                name: "SaleOrderLinePartnerCommissions");

            migrationBuilder.DropTable(
                name: "SaleOrderLinePaymentRels");

            migrationBuilder.DropTable(
                name: "Commissions");

            migrationBuilder.DropIndex(
                name: "IX_Employees_CommissionId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EmployeeId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CommissionId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "AspNetUsers");
        }
    }
}
