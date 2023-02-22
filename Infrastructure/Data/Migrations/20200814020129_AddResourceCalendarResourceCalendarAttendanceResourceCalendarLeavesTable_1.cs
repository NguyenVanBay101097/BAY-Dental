using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class AddResourceCalendarResourceCalendarAttendanceResourceCalendarLeavesTable_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HrPayrollStructures_Companies_CompanyId",
                table: "HrPayrollStructures");

            migrationBuilder.DropIndex(
                name: "IX_HrPayrollStructures_CompanyId",
                table: "HrPayrollStructures");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "HrPayrollStructures");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "HrPayrollStructures",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "HrPayrollStructures",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RegularPay",
                table: "HrPayrollStructures",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TypeId",
                table: "HrPayrollStructures",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "UseWorkedDayLines",
                table: "HrPayrollStructures",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "HourlyWage",
                table: "Employees",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StructureTypeId",
                table: "Employees",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Wage",
                table: "Employees",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ResourceCalendars",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: true),
                    HoursPerDay = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceCalendars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceCalendars_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResourceCalendars_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResourceCalendars_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HrPayrollStructureTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    DefaultResourceCalendarId = table.Column<Guid>(nullable: true),
                    DefaultSchedulePay = table.Column<string>(nullable: true),
                    DefaultStructId = table.Column<Guid>(nullable: true),
                    DefaultWorkEntryTypeId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    WageType = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrPayrollStructureTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrPayrollStructureTypes_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrPayrollStructureTypes_ResourceCalendars_DefaultResourceCalendarId",
                        column: x => x.DefaultResourceCalendarId,
                        principalTable: "ResourceCalendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrPayrollStructureTypes_HrPayrollStructures_DefaultStructId",
                        column: x => x.DefaultStructId,
                        principalTable: "HrPayrollStructures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrPayrollStructureTypes_WorkEntryTypes_DefaultWorkEntryTypeId",
                        column: x => x.DefaultWorkEntryTypeId,
                        principalTable: "WorkEntryTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HrPayrollStructureTypes_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResourceCalendarAttendances",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    DayOfWeek = table.Column<string>(nullable: false),
                    DateFrom = table.Column<DateTime>(nullable: true),
                    DateTo = table.Column<DateTime>(nullable: true),
                    HourFrom = table.Column<double>(nullable: false),
                    HourTo = table.Column<double>(nullable: false),
                    CalendarId = table.Column<Guid>(nullable: false),
                    DayPeriod = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceCalendarAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceCalendarAttendances_ResourceCalendars_CalendarId",
                        column: x => x.CalendarId,
                        principalTable: "ResourceCalendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceCalendarAttendances_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResourceCalendarAttendances_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResourceCalendarLeaves",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedById = table.Column<string>(nullable: true),
                    WriteById = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    CalendarId = table.Column<Guid>(nullable: true),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceCalendarLeaves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceCalendarLeaves_ResourceCalendars_CalendarId",
                        column: x => x.CalendarId,
                        principalTable: "ResourceCalendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResourceCalendarLeaves_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResourceCalendarLeaves_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResourceCalendarLeaves_AspNetUsers_WriteById",
                        column: x => x.WriteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HrPayrollStructures_TypeId",
                table: "HrPayrollStructures",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_StructureTypeId",
                table: "Employees",
                column: "StructureTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayrollStructureTypes_CreatedById",
                table: "HrPayrollStructureTypes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayrollStructureTypes_DefaultResourceCalendarId",
                table: "HrPayrollStructureTypes",
                column: "DefaultResourceCalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayrollStructureTypes_DefaultStructId",
                table: "HrPayrollStructureTypes",
                column: "DefaultStructId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayrollStructureTypes_DefaultWorkEntryTypeId",
                table: "HrPayrollStructureTypes",
                column: "DefaultWorkEntryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HrPayrollStructureTypes_WriteById",
                table: "HrPayrollStructureTypes",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceCalendarAttendances_CalendarId",
                table: "ResourceCalendarAttendances",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceCalendarAttendances_CreatedById",
                table: "ResourceCalendarAttendances",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceCalendarAttendances_WriteById",
                table: "ResourceCalendarAttendances",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceCalendarLeaves_CalendarId",
                table: "ResourceCalendarLeaves",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceCalendarLeaves_CompanyId",
                table: "ResourceCalendarLeaves",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceCalendarLeaves_CreatedById",
                table: "ResourceCalendarLeaves",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceCalendarLeaves_WriteById",
                table: "ResourceCalendarLeaves",
                column: "WriteById");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceCalendars_CompanyId",
                table: "ResourceCalendars",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceCalendars_CreatedById",
                table: "ResourceCalendars",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceCalendars_WriteById",
                table: "ResourceCalendars",
                column: "WriteById");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_HrPayrollStructureTypes_StructureTypeId",
                table: "Employees",
                column: "StructureTypeId",
                principalTable: "HrPayrollStructureTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HrPayrollStructures_HrPayrollStructureTypes_TypeId",
                table: "HrPayrollStructures",
                column: "TypeId",
                principalTable: "HrPayrollStructureTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_HrPayrollStructureTypes_StructureTypeId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_HrPayrollStructures_HrPayrollStructureTypes_TypeId",
                table: "HrPayrollStructures");

            migrationBuilder.DropTable(
                name: "HrPayrollStructureTypes");

            migrationBuilder.DropTable(
                name: "ResourceCalendarAttendances");

            migrationBuilder.DropTable(
                name: "ResourceCalendarLeaves");

            migrationBuilder.DropTable(
                name: "ResourceCalendars");

            migrationBuilder.DropIndex(
                name: "IX_HrPayrollStructures_TypeId",
                table: "HrPayrollStructures");

            migrationBuilder.DropIndex(
                name: "IX_Employees_StructureTypeId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "HrPayrollStructures");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "HrPayrollStructures");

            migrationBuilder.DropColumn(
                name: "RegularPay",
                table: "HrPayrollStructures");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "HrPayrollStructures");

            migrationBuilder.DropColumn(
                name: "UseWorkedDayLines",
                table: "HrPayrollStructures");

            migrationBuilder.DropColumn(
                name: "HourlyWage",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "StructureTypeId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Wage",
                table: "Employees");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "HrPayrollStructures",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_HrPayrollStructures_CompanyId",
                table: "HrPayrollStructures",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_HrPayrollStructures_Companies_CompanyId",
                table: "HrPayrollStructures",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
