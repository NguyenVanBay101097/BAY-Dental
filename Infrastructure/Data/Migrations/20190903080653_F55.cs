using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class F55 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_Employees_EmployeeBSId",
                table: "DotKhams");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_Employees_EmployeeKHId",
                table: "DotKhams");

            migrationBuilder.RenameColumn(
                name: "EmployeeKHId",
                table: "DotKhams",
                newName: "DoctorId");

            migrationBuilder.RenameColumn(
                name: "EmployeeBSId",
                table: "DotKhams",
                newName: "AssistantId");

            migrationBuilder.RenameIndex(
                name: "IX_DotKhams_EmployeeKHId",
                table: "DotKhams",
                newName: "IX_DotKhams_DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_DotKhams_EmployeeBSId",
                table: "DotKhams",
                newName: "IX_DotKhams_AssistantId");

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_Employees_AssistantId",
                table: "DotKhams",
                column: "AssistantId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_Employees_DoctorId",
                table: "DotKhams",
                column: "DoctorId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_Employees_AssistantId",
                table: "DotKhams");

            migrationBuilder.DropForeignKey(
                name: "FK_DotKhams_Employees_DoctorId",
                table: "DotKhams");

            migrationBuilder.RenameColumn(
                name: "DoctorId",
                table: "DotKhams",
                newName: "EmployeeKHId");

            migrationBuilder.RenameColumn(
                name: "AssistantId",
                table: "DotKhams",
                newName: "EmployeeBSId");

            migrationBuilder.RenameIndex(
                name: "IX_DotKhams_DoctorId",
                table: "DotKhams",
                newName: "IX_DotKhams_EmployeeKHId");

            migrationBuilder.RenameIndex(
                name: "IX_DotKhams_AssistantId",
                table: "DotKhams",
                newName: "IX_DotKhams_EmployeeBSId");

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_Employees_EmployeeBSId",
                table: "DotKhams",
                column: "EmployeeBSId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DotKhams_Employees_EmployeeKHId",
                table: "DotKhams",
                column: "EmployeeKHId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
