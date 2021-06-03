using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddModuleHangThanhVienTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MemberLevelId",
                table: "Partners",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Point",
                table: "Partners",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MemberLevels",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Point = table.Column<decimal>(nullable: false),
                    Color = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberLevels_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberLevels_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberLevels_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaleCouponProgramMemberLevelRels",
                columns: table => new
                {
                    ProgramId = table.Column<Guid>(nullable: false),
                    MemberLevelId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleCouponProgramMemberLevelRels", x => new { x.ProgramId, x.MemberLevelId });
                    table.ForeignKey(
                        name: "FK_SaleCouponProgramMemberLevelRels_MemberLevels_MemberLevelId",
                        column: x => x.MemberLevelId,
                        principalTable: "MemberLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleCouponProgramMemberLevelRels_SaleCouponPrograms_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "SaleCouponPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Partners_MemberLevelId",
                table: "Partners",
                column: "MemberLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberLevels_CompanyId",
                table: "MemberLevels",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberLevels_CreatedById",
                table: "MemberLevels",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MemberLevels_WriteById",
                table: "MemberLevels",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleCouponProgramMemberLevelRels_MemberLevelId",
                table: "SaleCouponProgramMemberLevelRels",
                column: "MemberLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_MemberLevels_MemberLevelId",
                table: "Partners",
                column: "MemberLevelId",
                principalTable: "MemberLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partners_MemberLevels_MemberLevelId",
                table: "Partners");

            migrationBuilder.DropTable(
                name: "SaleCouponProgramMemberLevelRels");

            migrationBuilder.DropTable(
                name: "MemberLevels");

            migrationBuilder.DropIndex(
                name: "IX_Partners_MemberLevelId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "MemberLevelId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "Point",
                table: "Partners");
        }
    }
}
