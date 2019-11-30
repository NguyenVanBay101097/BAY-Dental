using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F95 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NbPeriod",
                table: "CardTypes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Period",
                table: "CardTypes",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiredDate",
                table: "CardCards",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PointInPeriod",
                table: "CardCards",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CardHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    CardId = table.Column<Guid>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    PointInPeriod = table.Column<decimal>(nullable: true),
                    TotalPoint = table.Column<decimal>(nullable: true),
                    TypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardHistories_CardCards_CardId",
                        column: x => x.CardId,
                        principalTable: "CardCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardHistories_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CardHistories_CardTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "CardTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CardHistories_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardHistories_CardId",
                table: "CardHistories",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_CardHistories_CreatedById",
                table: "CardHistories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CardHistories_TypeId",
                table: "CardHistories",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CardHistories_WriteById",
                table: "CardHistories",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardHistories");

            migrationBuilder.DropColumn(
                name: "NbPeriod",
                table: "CardTypes");

            migrationBuilder.DropColumn(
                name: "Period",
                table: "CardTypes");

            migrationBuilder.DropColumn(
                name: "ExpiredDate",
                table: "CardCards");

            migrationBuilder.DropColumn(
                name: "PointInPeriod",
                table: "CardCards");
        }
    }
}
