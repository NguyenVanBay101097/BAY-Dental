using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F64 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerHistoryRels_Partners_HistoryId",
                table: "PartnerHistoryRels");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerHistoryRels_Partners_PartnerId",
                table: "PartnerHistoryRels",
                column: "PartnerId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartnerHistoryRels_Partners_PartnerId",
                table: "PartnerHistoryRels");

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerHistoryRels_Partners_HistoryId",
                table: "PartnerHistoryRels",
                column: "HistoryId",
                principalTable: "Partners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
