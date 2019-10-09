using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F63 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_History_AspNetUsers_CreatedById",
                table: "History");

            migrationBuilder.DropForeignKey(
                name: "FK_History_AspNetUsers_WriteById",
                table: "History");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerHistoryRels_History_HistoryId",
                table: "PartnerHistoryRels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_History",
                table: "History");

            migrationBuilder.RenameTable(
                name: "History",
                newName: "Histories");

            migrationBuilder.RenameIndex(
                name: "IX_History_WriteById",
                table: "Histories",
                newName: "IX_Histories_WriteById");

            migrationBuilder.RenameIndex(
                name: "IX_History_CreatedById",
                table: "Histories",
                newName: "IX_Histories_CreatedById");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Histories",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Histories",
                table: "Histories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Histories_AspNetUsers_CreatedById",
                table: "Histories",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Histories_AspNetUsers_WriteById",
                table: "Histories",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerHistoryRels_Histories_HistoryId",
                table: "PartnerHistoryRels",
                column: "HistoryId",
                principalTable: "Histories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Histories_AspNetUsers_CreatedById",
                table: "Histories");

            migrationBuilder.DropForeignKey(
                name: "FK_Histories_AspNetUsers_WriteById",
                table: "Histories");

            migrationBuilder.DropForeignKey(
                name: "FK_PartnerHistoryRels_Histories_HistoryId",
                table: "PartnerHistoryRels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Histories",
                table: "Histories");

            migrationBuilder.RenameTable(
                name: "Histories",
                newName: "History");

            migrationBuilder.RenameIndex(
                name: "IX_Histories_WriteById",
                table: "History",
                newName: "IX_History_WriteById");

            migrationBuilder.RenameIndex(
                name: "IX_Histories_CreatedById",
                table: "History",
                newName: "IX_History_CreatedById");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "History",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_History",
                table: "History",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_History_AspNetUsers_CreatedById",
                table: "History",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_History_AspNetUsers_WriteById",
                table: "History",
                column: "WriteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PartnerHistoryRels_History_HistoryId",
                table: "PartnerHistoryRels",
                column: "HistoryId",
                principalTable: "History",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
