using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class CreateTable_LoaiThuChi_PhieuThuChi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PhieuThuChiId",
                table: "AccountMoveLines",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LoaiThuChis",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    IsInclude = table.Column<bool>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiThuChis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoaiThuChis_AccountAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "AccountAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoaiThuChis_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoaiThuChis_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoaiThuChis_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhieuThuChis",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    JournalId = table.Column<Guid>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    Communication = table.Column<string>(nullable: true),
                    Reason = table.Column<string>(nullable: true),
                    PayerReceiver = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    LoaiThuChiId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuThuChis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhieuThuChis_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuThuChis_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuThuChis_AccountJournals_JournalId",
                        column: x => x.JournalId,
                        principalTable: "AccountJournals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuThuChis_LoaiThuChis_LoaiThuChiId",
                        column: x => x.LoaiThuChiId,
                        principalTable: "LoaiThuChis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhieuThuChis_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoveLines_PhieuThuChiId",
                table: "AccountMoveLines",
                column: "PhieuThuChiId");

            migrationBuilder.CreateIndex(
                name: "IX_LoaiThuChis_AccountId",
                table: "LoaiThuChis",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_LoaiThuChis_CompanyId",
                table: "LoaiThuChis",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_LoaiThuChis_CreatedById",
                table: "LoaiThuChis",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LoaiThuChis_WriteById",
                table: "LoaiThuChis",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThuChis_CompanyId",
                table: "PhieuThuChis",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThuChis_CreatedById",
                table: "PhieuThuChis",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThuChis_JournalId",
                table: "PhieuThuChis",
                column: "JournalId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThuChis_LoaiThuChiId",
                table: "PhieuThuChis",
                column: "LoaiThuChiId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuThuChis_WriteById",
                table: "PhieuThuChis",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_PhieuThuChis_PhieuThuChiId",
                table: "AccountMoveLines",
                column: "PhieuThuChiId",
                principalTable: "PhieuThuChis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountMoveLines_PhieuThuChis_PhieuThuChiId",
                table: "AccountMoveLines");

            migrationBuilder.DropTable(
                name: "PhieuThuChis");

            migrationBuilder.DropTable(
                name: "LoaiThuChis");

            migrationBuilder.DropIndex(
                name: "IX_AccountMoveLines_PhieuThuChiId",
                table: "AccountMoveLines");

            migrationBuilder.DropColumn(
                name: "PhieuThuChiId",
                table: "AccountMoveLines");
        }
    }
}
