using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddColumnSalesmanIdAccountMoveLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SalesmanId",
                table: "AccountMoveLines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountMoveLines_SalesmanId",
                table: "AccountMoveLines",
                column: "SalesmanId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountMoveLines_AspNetUsers_SalesmanId",
                table: "AccountMoveLines",
                column: "SalesmanId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountMoveLines_AspNetUsers_SalesmanId",
                table: "AccountMoveLines");

            migrationBuilder.DropIndex(
                name: "IX_AccountMoveLines_SalesmanId",
                table: "AccountMoveLines");

            migrationBuilder.DropColumn(
                name: "SalesmanId",
                table: "AccountMoveLines");
        }
    }
}
