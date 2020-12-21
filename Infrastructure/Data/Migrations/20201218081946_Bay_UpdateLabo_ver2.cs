using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_UpdateLabo_ver2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CategId",
                table: "Products",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateExport",
                table: "LaboOrders",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateReceipt",
                table: "LaboOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Indicated",
                table: "LaboOrders",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LaboBiteJointId",
                table: "LaboOrders",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LaboBridgeId",
                table: "LaboOrders",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LaboFinishLineId",
                table: "LaboOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TechnicalNote",
                table: "LaboOrders",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LaboBiteJoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboBiteJoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaboBiteJoints_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboBiteJoints_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LaboBridges",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboBridges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaboBridges_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboBridges_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LaboFinishLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboFinishLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LaboFinishLines_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LaboFinishLines_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "laboOrderProductRels",
                columns: table => new
                {
                    LaboOrderId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_laboOrderProductRels", x => new { x.LaboOrderId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_laboOrderProductRels_LaboOrders_LaboOrderId",
                        column: x => x.LaboOrderId,
                        principalTable: "LaboOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_laboOrderProductRels_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_LaboBiteJointId",
                table: "LaboOrders",
                column: "LaboBiteJointId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_LaboBridgeId",
                table: "LaboOrders",
                column: "LaboBridgeId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboOrders_LaboFinishLineId",
                table: "LaboOrders",
                column: "LaboFinishLineId");

            migrationBuilder.CreateIndex(
                name: "IX_LaboBiteJoints_CreatedById",
                table: "LaboBiteJoints",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LaboBiteJoints_WriteById",
                table: "LaboBiteJoints",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_LaboBridges_CreatedById",
                table: "LaboBridges",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LaboBridges_WriteById",
                table: "LaboBridges",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_LaboFinishLines_CreatedById",
                table: "LaboFinishLines",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LaboFinishLines_WriteById",
                table: "LaboFinishLines",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_laboOrderProductRels_ProductId",
                table: "laboOrderProductRels",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrders_LaboBiteJoints_LaboBiteJointId",
                table: "LaboOrders",
                column: "LaboBiteJointId",
                principalTable: "LaboBiteJoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrders_LaboBridges_LaboBridgeId",
                table: "LaboOrders",
                column: "LaboBridgeId",
                principalTable: "LaboBridges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LaboOrders_LaboFinishLines_LaboFinishLineId",
                table: "LaboOrders",
                column: "LaboFinishLineId",
                principalTable: "LaboFinishLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaboOrders_LaboBiteJoints_LaboBiteJointId",
                table: "LaboOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_LaboOrders_LaboBridges_LaboBridgeId",
                table: "LaboOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_LaboOrders_LaboFinishLines_LaboFinishLineId",
                table: "LaboOrders");

            migrationBuilder.DropTable(
                name: "LaboBiteJoints");

            migrationBuilder.DropTable(
                name: "LaboBridges");

            migrationBuilder.DropTable(
                name: "LaboFinishLines");

            migrationBuilder.DropTable(
                name: "laboOrderProductRels");

            migrationBuilder.DropIndex(
                name: "IX_LaboOrders_LaboBiteJointId",
                table: "LaboOrders");

            migrationBuilder.DropIndex(
                name: "IX_LaboOrders_LaboBridgeId",
                table: "LaboOrders");

            migrationBuilder.DropIndex(
                name: "IX_LaboOrders_LaboFinishLineId",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "DateExport",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "DateReceipt",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "Indicated",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "LaboBiteJointId",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "LaboBridgeId",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "LaboFinishLineId",
                table: "LaboOrders");

            migrationBuilder.DropColumn(
                name: "TechnicalNote",
                table: "LaboOrders");

            migrationBuilder.AlterColumn<Guid>(
                name: "CategId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
