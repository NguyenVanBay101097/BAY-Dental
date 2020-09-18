using Microsoft.EntityFrameworkCore.Migrations;
using ApplicationCore.Utilities;
using MyERP.Utilities;
using System.Collections.Generic;
using ApplicationCore.Entities;
using System.Linq;

namespace Infrastructure.Data.Migrations
{
    public partial class InsertDataIrModelRuleSalary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var models = new[]
            {
                new { Id = GuidComb.GenerateComb(), Name = "Đợt lương", Model = "HrPayslipRun", Transient = 0, IdStr = "hr.model_payslip_run" },
                new { Id = GuidComb.GenerateComb(), Name = "Phiếu lương", Model = "HrPayslip", Transient = 0, IdStr = "hr.model_payslip" },
                new { Id = GuidComb.GenerateComb(), Name = "Chấm công", Model = "ChamCong", Transient = 0, IdStr = "hr.model_cham_cong" },
                new { Id = GuidComb.GenerateComb(), Name = "Loại mẫu lương", Model = "HrPayrollStructureType", Transient = 0, IdStr = "hr.model_payroll_structure_type" },
                new { Id = GuidComb.GenerateComb(), Name = "Mẫu lương", Model = "HrPayrollStructure", Transient = 0, IdStr = "hr.model_payroll_structure" },
                new { Id = GuidComb.GenerateComb(), Name = "Loại chấm công", Model = "WorkEntryType", Transient = 0, IdStr = "hr.model_work_entry_type" },
                new { Id = GuidComb.GenerateComb(), Name = "Thời gian làm việc", Model = "ResourceCalendar", Transient = 0, IdStr = "base.model_resource_calendar" },
            };

            var model_dict = models.ToDictionary(x => x.IdStr, x => x.Id);

            foreach (var model in models)
                migrationBuilder.Sql($"INSERT INTO IRModels(Id, Name, Model, Transient) Values('{model.Id}', N'{model.Name}', '{model.Model}', {model.Transient})");

            var rules = new[]
            {
                new { Id = GuidComb.GenerateComb(), Name = "Cham cong multi-company", ModelId = "hr.model_cham_cong", Code = "hr.cham_cong_comp_rule" },
                new { Id = GuidComb.GenerateComb(), Name = "Payslip Run multi-company", ModelId = "hr.model_payslip_run", Code = "hr.payslip_run_comp_rule" },
                new { Id = GuidComb.GenerateComb(), Name = "Payslip multi-company", ModelId = "hr.model_payslip", Code = "hr.payslip_comp_rule" },
                new { Id = GuidComb.GenerateComb(), Name = "Payroll Structure multi-company", ModelId = "hr.model_payroll_structure", Code = "hr.payroll_structure_comp_rule" },
                new { Id = GuidComb.GenerateComb(), Name = "Payroll Structure Type multi-company", ModelId = "hr.model_payroll_structure_type", Code = "hr.payroll_structure_type_comp_rule" },
                new { Id = GuidComb.GenerateComb(), Name = "Resource Calendar multi-company", ModelId = "base.model_resource_calendar", Code = "base.resource_calendar_comp_rule" },
            };

            foreach (var rule in rules)
                migrationBuilder.Sql($"INSERT INTO IRRules(Id,Name,Global,Active,PermUnlink,PermWrite,PermRead,PermCreate,ModelId,Code) Values('{rule.Id}',N'{rule.Name}',1,1,1,1,1,1,'{model_dict[rule.ModelId]}','{rule.Code}')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var rules = new string[] {
                "hr.cham_cong_comp_rule",
                "hr.payslip_run_comp_rule",
                "hr.payslip_comp_rule",
                "hr.payroll_structure_comp_rule",
                "hr.payroll_structure_type_comp_rule",
                "base.resource_calendar_comp_rule" };

            foreach (var rule in rules)
                migrationBuilder.Sql($"DELETE IRRules WHERE Code='{rule}'");

            var models = new string[] {
                "HrPayslipRun",
                "HrPayslip",
                "ChamCong",
                "HrPayrollStructureType",
                "HrPayrollStructure",
                "ResourceCalendar",
                "WorkEntryType"
            };

            foreach (var model in models)
                migrationBuilder.Sql($"DELETE IRModels WHERE Model='{model}'");
        }
    }
}
