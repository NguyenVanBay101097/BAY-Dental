using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyAssignmentsController : BaseApiController
    {
        private readonly ISurveyAssignmentService _surveyAssignmentService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly ISaleOrderService _saleOrderService;
        private readonly IEmployeeService _employeeService;

        public SurveyAssignmentsController(
            ISurveyAssignmentService surveyAssignmentService,
            IMapper mapper,
            IUnitOfWorkAsync unitOfWork, ISaleOrderService saleOrderService,
            IEmployeeService employeeService
            )
        {
            _surveyAssignmentService = surveyAssignmentService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _saleOrderService = saleOrderService;
            _employeeService = employeeService;
        }

        [HttpGet]
        [CheckAccess(Actions="Survey.Assignment.Read")]
        public async Task<IActionResult> Get([FromQuery] SurveyAssignmentPaged val)
        {
            var result = await _surveyAssignmentService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Survey.Assignment.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _surveyAssignmentService.GetDisplay(id);
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Survey.Assignment.Create")]
        public async Task<IActionResult> Create(SurveyAssignmentSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();

            var assignment = _mapper.Map<SurveyAssignment>(val);
            var saleOrder = await _saleOrderService.GetByIdAsync(val.SaleOrderId);
            assignment.CompanyId = saleOrder.CompanyId;
            assignment.PartnerId = saleOrder.PartnerId;

            var employee = await _employeeService.GetByIdAsync(val.EmployeeId);
            assignment.UserId = employee.UserId;
            await _surveyAssignmentService.CreateAsync(assignment);

            _unitOfWork.Commit();
            return Ok(_mapper.Map<SurveyAssignmentGridItem>(assignment));
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Survey.Assignment.Create")]
        public async Task<IActionResult> CreateList(IEnumerable<SurveyAssignmentSave> vals)
        {
            if (vals.Count() == 0 || !ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();

            var assignments = _mapper.Map<IEnumerable<SurveyAssignment>>(vals);
            foreach(var assignment in assignments)
            {
                var saleOrder = await _saleOrderService.GetByIdAsync(assignment.SaleOrderId);
                assignment.CompanyId = saleOrder.CompanyId;
                assignment.PartnerId = saleOrder.PartnerId;

                var employee = await _employeeService.GetByIdAsync(assignment.EmployeeId);
                assignment.UserId = employee.UserId;
            }

            await _surveyAssignmentService.CreateAsync(assignments);

            _unitOfWork.Commit();
            return Ok(vals);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Survey.Assignment.Update")]
        public async Task<IActionResult> Update(Guid id, SurveyAssignmentSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();

            var assign = await _surveyAssignmentService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (assign == null)
                return NotFound();

            if (val.EmployeeId != assign.EmployeeId) assign.AssignDate = DateTime.Now;

            assign = _mapper.Map(val, assign);

            await _surveyAssignmentService.UpdateAsync(assign);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Survey.Assignment.Update")]
        public async Task<IActionResult> ActionContact(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();
           
            await _unitOfWork.BeginTransactionAsync();
            await _surveyAssignmentService.ActionContact(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Survey.Assignment.DeleteUserInput")]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _surveyAssignmentService.ActionCancel(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        //[HttpPost("[action]")]
        //[CheckAccess(Actions = "Survey.Assignment.Done")]
        //public async Task<IActionResult> ActionDone(AssignmentActionDone val)
        //{
        //    await _unitOfWork.BeginTransactionAsync();
        //    await _surveyAssignmentService.ActionDone(val);
        //    _unitOfWork.Commit();
        //    return NoContent();
        //}

        [HttpPatch("{id}")]
        [CheckAccess(Actions = "Survey.Assignment.Update")]
        public async Task<IActionResult> JsonPatchWithModelState(Guid id, [FromBody]JsonPatchDocument<SurveyAssignmentPatch> patchDoc)
        {
            var entity = await _surveyAssignmentService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            var entityMap = _mapper.Map<SurveyAssignmentPatch>(entity);
            patchDoc.ApplyTo(entityMap, ModelState);
            _mapper.Map(entityMap, entity);
            await _unitOfWork.BeginTransactionAsync();
            await _surveyAssignmentService.UpdateAsync(entity);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Survey.Assignment.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            var assign = await _surveyAssignmentService.GetByIdAsync(id);

            if (assign == null)
                return NotFound();

            await _surveyAssignmentService.DeleteAsync(assign);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Survey.Assignment.Read")]
        public async Task<IActionResult> DefaultGetList(SurveyAssignmentDefaultGetPar val)
        {
            var result = await _surveyAssignmentService.DefaultGetList(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Survey.Assignment.Read")]
        public async Task<IActionResult> GetSummary(SurveyAssignmentGetSummaryFilter val)
        {
            var result = await _surveyAssignmentService.GetSummary(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Survey.Assignment.Update")]
        public async Task<IActionResult> UpdateEmployee(SurveyAssignmentUpdateEmployee val)
        {
            var assignment = await _surveyAssignmentService.GetByIdAsync(val.Id);
            if (assignment == null)
                return NotFound();
            if (assignment.Status == "done")
                throw new Exception("Phân việc ở trạng thái hoàn thành không thể đổi nhân viên");

            assignment.EmployeeId = val.EmployeeId;
            var employee = await _employeeService.GetByIdAsync(assignment.EmployeeId);
            assignment.UserId = employee.UserId;

            await _surveyAssignmentService.UpdateAsync(assignment);
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Survey.Assignment.Read")]
        public async Task<IActionResult> ExportDoneSurveyAssignmentExcel([FromBody] SurveyAssignmentPaged val)
        {
            var result = await _surveyAssignmentService.GetPagedResultAsync(val);
            var data = result.Items;
            var stream = new MemoryStream();
            var sheetName = "DanhSachKhaoSatHoanThanh";
            byte[] fileContent;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);
                worksheet.Cells["A1:H1"].Value = "DANH SÁCH KHẢO SÁT HOÀN THÀNH";
                worksheet.Cells["A1:H1"].Style.Font.Size = 14;
                worksheet.Cells["A1:H1"].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#6ca4cc"));
                worksheet.Cells["A1:H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:H1"].Merge = true;
                worksheet.Cells["A1:H1"].Style.Font.Bold = true;
                worksheet.Cells["A2:H2"].Value = $"Từ ngày {val.DateFrom.Value.ToString("dd/MM/yyyy")} đến ngày {val.DateTo.Value.ToString("dd/MM/yyyy")}"; ;
                worksheet.Cells["A2:H2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:H2"].Merge = true;
                worksheet.Cells["A3:H3"].Value = "";

                var row = 4;
                worksheet.Cells[row, 1].Value = "Khách hàng";
                worksheet.Cells[row, 2].Value = "Tuổi";
                worksheet.Cells[row, 3].Value = "Phiếu điều trị";
                worksheet.Cells[row, 4].Value = "Nhân viên khảo sát";
                worksheet.Cells[row, 5].Value = "Ngày phân việc";
                worksheet.Cells[row, 6].Value = "Ngày hoàn thành";
                worksheet.Cells[row, 7].Value = "Nhãn khảo sát";
                worksheet.Cells[row, 8].Value = "Điểm khảo sát";
                worksheet.Cells["A1:H1"].Style.Font.Bold = true;
                worksheet.Cells["A4:H4"].Style.Font.Bold = true;

                row = 5;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.PartnerName;
                    worksheet.Cells[row, 2].Value = item.Age;
                    worksheet.Cells[row, 3].Value = item.SaleOrder.Name;
                    worksheet.Cells[row, 4].Value = item.Employee.Name;
                    worksheet.Cells[row, 5].Value = item.AssignDate;
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "dd/mm/yyyy";
                    worksheet.Cells[row, 6].Value = item.CompleteDate;
                    worksheet.Cells[row, 6].Style.Numberformat.Format = "dd/mm/yyyy";
                    worksheet.Cells[row, 7].Value = item.SurveyTags;
                    worksheet.Cells[row, 8].Value = (item.UserInputScore.HasValue && item.UserInputMaxScore.HasValue) ? item.UserInputScore.Value+"/"+item.UserInputMaxScore.Value : "";
                    row++;
                }

                worksheet.Cells.AutoFitColumns();

                package.Save();

                fileContent = stream.ToArray();
            }

            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            stream.Position = 0;

            return new FileContentResult(fileContent, mimeType);
        }
    }
}
