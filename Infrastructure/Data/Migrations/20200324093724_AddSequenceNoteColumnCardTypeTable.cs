using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddSequenceNoteColumnCardTypeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "CardTypes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "CardTypes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "CardTypes");

            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "CardTypes");
        }
    }
}
