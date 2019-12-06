using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F99 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PromotionPrograms",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    DateFrom = table.Column<DateTime>(nullable: true),
                    DateTo = table.Column<DateTime>(nullable: true),
                    MaximumUseNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionPrograms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionPrograms_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromotionPrograms_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PromotionProgramCompanyRels",
                columns: table => new
                {
                    PromotionProgramId = table.Column<Guid>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionProgramCompanyRels", x => new { x.PromotionProgramId, x.CompanyId });
                    table.ForeignKey(
                        name: "FK_PromotionProgramCompanyRels_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromotionProgramCompanyRels_PromotionPrograms_PromotionProgramId",
                        column: x => x.PromotionProgramId,
                        principalTable: "PromotionPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromotionRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    ProgramId = table.Column<Guid>(nullable: false),
                    MinQuantity = table.Column<decimal>(nullable: true),
                    DiscountType = table.Column<string>(nullable: true),
                    DiscountPercentage = table.Column<decimal>(nullable: true),
                    DiscountFixedAmount = table.Column<decimal>(nullable: true),
                    DiscountApplyOn = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionRules_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromotionRules_PromotionPrograms_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "PromotionPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromotionRules_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PromotionRuleProductCategoryRels",
                columns: table => new
                {
                    RuleId = table.Column<Guid>(nullable: false),
                    CategId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionRuleProductCategoryRels", x => new { x.RuleId, x.CategId });
                    table.ForeignKey(
                        name: "FK_PromotionRuleProductCategoryRels_ProductCategories_CategId",
                        column: x => x.CategId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromotionRuleProductCategoryRels_PromotionRules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "PromotionRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromotionRuleProductRels",
                columns: table => new
                {
                    RuleId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionRuleProductRels", x => new { x.RuleId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_PromotionRuleProductRels_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromotionRuleProductRels_PromotionRules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "PromotionRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PromotionProgramCompanyRels_CompanyId",
                table: "PromotionProgramCompanyRels",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionPrograms_CreatedById",
                table: "PromotionPrograms",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionPrograms_WriteById",
                table: "PromotionPrograms",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRuleProductCategoryRels_CategId",
                table: "PromotionRuleProductCategoryRels",
                column: "CategId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRuleProductRels_ProductId",
                table: "PromotionRuleProductRels",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRules_CreatedById",
                table: "PromotionRules",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRules_ProgramId",
                table: "PromotionRules",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionRules_WriteById",
                table: "PromotionRules",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromotionProgramCompanyRels");

            migrationBuilder.DropTable(
                name: "PromotionRuleProductCategoryRels");

            migrationBuilder.DropTable(
                name: "PromotionRuleProductRels");

            migrationBuilder.DropTable(
                name: "PromotionRules");

            migrationBuilder.DropTable(
                name: "PromotionPrograms");
        }
    }
}
