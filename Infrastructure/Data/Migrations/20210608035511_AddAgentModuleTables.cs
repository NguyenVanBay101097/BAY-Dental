using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddAgentModuleTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partners_Employees_ConsultantId",
                table: "Partners");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuThuChis_LoaiThuChis_LoaiThuChiId",
                table: "PhieuThuChis");

            migrationBuilder.DropIndex(
                name: "IX_Partners_ConsultantId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "ConsultantId",
                table: "Partners");

            migrationBuilder.AlterColumn<Guid>(
                name: "LoaiThuChiId",
                table: "PhieuThuChis",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "PhieuThuChis",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountType",
                table: "PhieuThuChis",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AgentId",
                table: "PhieuThuChis",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AgentId",
                table: "Partners",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAgent",
                table: "Partners",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "PartnerId",
                table: "CommissionSettlements",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Agents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Gender = table.Column<string>(nullable: true),
                    BirthYear = table.Column<int>(nullable: true),
                    BirthMonth = table.Column<int>(nullable: true),
                    BirthDay = table.Column<int>(nullable: true),
                    JobTitle = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    PartnerId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Agents_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Agents_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Agents_Partners_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Agents_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThuChis_AccountId",
                table: "PhieuThuChis",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThuChis_AgentId",
                table: "PhieuThuChis",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_AgentId",
                table: "Partners",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionSettlements_PartnerId",
                table: "CommissionSettlements",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Agents_CompanyId",
                table: "Agents",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Agents_CreatedById",
                table: "Agents",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Agents_PartnerId",
                table: "Agents",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Agents_WriteById",
                table: "Agents",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_CommissionSettlements_Partners_PartnerId",
                table: "CommissionSettlements",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_Agents_AgentId",
                table: "Partners",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuThuChis_AccountAccounts_AccountId",
                table: "PhieuThuChis",
                column: "AccountId",
                principalTable: "AccountAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuThuChis_Agents_AgentId",
                table: "PhieuThuChis",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuThuChis_LoaiThuChis_LoaiThuChiId",
                table: "PhieuThuChis",
                column: "LoaiThuChiId",
                principalTable: "LoaiThuChis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommissionSettlements_Partners_PartnerId",
                table: "CommissionSettlements");

            migrationBuilder.DropForeignKey(
                name: "FK_Partners_Agents_AgentId",
                table: "Partners");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuThuChis_AccountAccounts_AccountId",
                table: "PhieuThuChis");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuThuChis_Agents_AgentId",
                table: "PhieuThuChis");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuThuChis_LoaiThuChis_LoaiThuChiId",
                table: "PhieuThuChis");

            migrationBuilder.DropTable(
                name: "Agents");

            migrationBuilder.DropIndex(
                name: "IX_PhieuThuChis_AccountId",
                table: "PhieuThuChis");

            migrationBuilder.DropIndex(
                name: "IX_PhieuThuChis_AgentId",
                table: "PhieuThuChis");

            migrationBuilder.DropIndex(
                name: "IX_Partners_AgentId",
                table: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_CommissionSettlements_PartnerId",
                table: "CommissionSettlements");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "PhieuThuChis");

            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "PhieuThuChis");

            migrationBuilder.DropColumn(
                name: "AgentId",
                table: "PhieuThuChis");

            migrationBuilder.DropColumn(
                name: "AgentId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "IsAgent",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "PartnerId",
                table: "CommissionSettlements");

            migrationBuilder.AlterColumn<Guid>(
                name: "LoaiThuChiId",
                table: "PhieuThuChis",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ConsultantId",
                table: "Partners",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Partners_ConsultantId",
                table: "Partners",
                column: "ConsultantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_Employees_ConsultantId",
                table: "Partners",
                column: "ConsultantId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuThuChis_LoaiThuChis_LoaiThuChiId",
                table: "PhieuThuChis",
                column: "LoaiThuChiId",
                principalTable: "LoaiThuChis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
