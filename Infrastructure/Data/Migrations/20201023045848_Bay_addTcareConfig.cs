using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class Bay_addTcareConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TCareConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    JobCampaignHour = table.Column<int>(nullable: true),
                    JobCampaignMinute = table.Column<int>(nullable: true),
                    JobMessagingMinute = table.Column<int>(nullable: true),
                    JobMessageMinute = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TCareConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TCareConfigs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TCareConfigs_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TCareConfigs_CreatedById",
                table: "TCareConfigs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TCareConfigs_WriteById",
                table: "TCareConfigs",
                column: "WriteById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TCareConfigs");
        }
    }
}
