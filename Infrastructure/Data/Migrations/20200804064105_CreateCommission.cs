using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CreateCommission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CommissionId",
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

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CommissionId",
                table: "AspNetUsers",
                column: "CommissionId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Commissions_CommissionId",
                table: "AspNetUsers",
                column: "CommissionId",
                principalTable: "Commissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Commissions_CommissionId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "CommissionProductRules");            

            migrationBuilder.DropTable(
                name: "Commissions");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CommissionId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CommissionId",
                table: "AspNetUsers");
        }
    }
}
