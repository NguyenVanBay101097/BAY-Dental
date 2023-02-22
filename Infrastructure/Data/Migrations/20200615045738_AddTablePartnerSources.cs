using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddTablePartnerSources : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partners_Employees_EmployeeId",
                table: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_Partners_EmployeeId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Partners");

            migrationBuilder.AddColumn<string>(
                name: "ReferralUserId",
                table: "Partners",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SourceId",
                table: "Partners",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PartnerSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerSources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartnerSources_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartnerSources_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Partners_ReferralUserId",
                table: "Partners",
                column: "ReferralUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_SourceId",
                table: "Partners",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerSources_CreatedById",
                table: "PartnerSources",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerSources_WriteById",
                table: "PartnerSources",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_AspNetUsers_ReferralUserId",
                table: "Partners",
                column: "ReferralUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_PartnerSources_SourceId",
                table: "Partners",
                column: "SourceId",
                principalTable: "PartnerSources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partners_AspNetUsers_ReferralUserId",
                table: "Partners");

            migrationBuilder.DropForeignKey(
                name: "FK_Partners_PartnerSources_SourceId",
                table: "Partners");

            migrationBuilder.DropTable(
                name: "PartnerSources");

            migrationBuilder.DropIndex(
                name: "IX_Partners_ReferralUserId",
                table: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_Partners_SourceId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "ReferralUserId",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Partners");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "Partners",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Partners",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Partners_EmployeeId",
                table: "Partners",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_Employees_EmployeeId",
                table: "Partners",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
