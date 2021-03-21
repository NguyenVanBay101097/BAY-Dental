using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HrPayslipRunsController : BaseApiController
    {
        private readonly IHrPayslipRunService _payslipRunService;
        private readonly IHrPayslipService _payslipServie;
        private readonly IMapper _mapper;
        private readonly IViewRenderService _view;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public HrPayslipRunsController(ICompanyService companyService, IHrPayslipRunService payslipRunService, IViewRenderService view, IMapper mapper, IUnitOfWorkAsync unitOfWork, IHrPayslipService payslipService)
        {
            _payslipRunService = payslipRunService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _view = view;
            _payslipServie = payslipService;
        }

        [HttpGet]
        [CheckAccess(Actions = "Salary.HrPayslipRun.Read")]
        public async Task<IActionResult> Get([FromQuery] HrPayslipRunPaged val)
        {
            var result = await _payslipRunService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Salary.HrPayslipRun.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _payslipRunService.GetHrPayslipRunForDisplay(id);
            return Ok(res);
        }


        [HttpPut("{id}/[action]")]
        [CheckAccess(Actions = "Salary.HrPayslipRun.Read")]
        public async Task<IActionResult> Print(Guid id, HrPayslipRunSave val)
        {
            var ids = val.Slips.Where(x => x.IsCheck == true).Select(x => x.Id);
            await _payslipRunService.UpdatePayslipRun(id, val);
            var res = await _payslipRunService.GetHrPayslipRunForDisplay(id);
            if (ids != null && ids.Any())
            {
                res.Slips = res.Slips.Where(x => ids.Contains(x.Id));
                var html = _view.Render("SalaryEmployee/Print", res);
                return Ok(new PrintData() { html = html });
            }
            else
            {
                return Ok(new PrintData() { html = null });
            }
        }

        [HttpPost]
        [CheckAccess(Actions = "Salary.HrPayslipRun.Create")]
        public async Task<IActionResult> Create(HrPayslipRunSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            var payslipRun = await _payslipRunService.CreatePayslipRun(val);

            _unitOfWork.Commit();

            var basic = _mapper.Map<HrPayslipRunBasic>(payslipRun);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Salary.HrPayslipRun.Update")]
        public async Task<IActionResult> Update(Guid id, HrPayslipRunSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _payslipRunService.UpdatePayslipRun(id, val);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]/{id}")]
        [CheckAccess(Actions = "Salary.HrPayslipRun.Update")]
        public async Task<IActionResult> ActionConfirm(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _payslipRunService.ActionConfirm(id);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]/{id}")]
        [CheckAccess(Actions = "Salary.HrPayslipRun.Update")]
        public async Task<IActionResult> CreatePayslipByRunId(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();

            var res = await _payslipRunService.CreatePayslipByRunId(id);

            _unitOfWork.Commit();

            return Ok(res);
        }

        [HttpPost("[action]/{id}")]
        [CheckAccess(Actions = "Salary.HrPayslipRun.Update")]
        public async Task<IActionResult> ComputeSalaryByRunId(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _payslipRunService.ComputeSalaryByRunId(id);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Salary.HrPayslipRun.Update")]
        public async Task<IActionResult> ActionDone(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _payslipRunService.ActionDone(ids);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Salary.HrPayslipRun.Cancel")]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _payslipRunService.ActionCancel(ids);

            _unitOfWork.Commit();

            return NoContent();
        }


        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Salary.HrPayslipRun.Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var paySlip = await _payslipRunService.SearchQuery(x => x.Id == id).Include(x => x.Slips).FirstOrDefaultAsync();
            if (paySlip == null)
                return BadRequest();

            if (paySlip.State != "confirm")
                throw new Exception("Đợt lương ở trạng thái chờ xác nhận mới có thể xóa");

            await _payslipRunService.DeleteAsync(paySlip);

            return NoContent();
        }

        [HttpPost("[action]")]
        public IActionResult DefaultGet(HrPayslipRunDefaultGet val)
        {
            var date = DateTime.Now;
            var startDate = new DateTime(date.Year, date.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var res = new HrPayslipRunSave();
            res.State = val.State;
            res.CompanyId = CompanyId;
            res.DateStart = startDate;
            res.DateEnd = endDate;
            res.Date = date;
            res.State = "confirm";

            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Salary.HrPayslipRun.Read")]
        public async Task<IActionResult> CheckExist([FromQuery] DateTime? date)
        {
            if (date == null)
            {
                throw new Exception("phải chọn ngày để kiểm tra bảng lương");
            }
            var res = await _payslipRunService.CheckExist(date.Value);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Salary.HrPayslipRun.Read")]
        public async Task<IActionResult> ExportExcelFile(IEnumerable<Guid> payslipIds)
        {
            CultureInfo cul = CultureInfo.CurrentCulture;
            string groupSep = cul.NumberFormat.CurrencyGroupSeparator;
            var stream = new MemoryStream();
            var data = await _mapper.ProjectTo<HrPayslipDisplay>(_payslipServie.SearchQuery(x => payslipIds.Contains(x.Id))
                .Include(x=>x.Employee).Include(x=>x.SalaryPayment)).ToListAsync();
            byte[] fileContent;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                worksheet.Cells[1, 1].Value = "Nhân viên";
                worksheet.Cells[1, 2].Value = "Lương ngày";
                worksheet.Cells[1, 3].Value = "Số ngày";
                worksheet.Cells[1, 4].Value = "Lương cơ bản";
                worksheet.Cells[1, 5].Value = "Số giờ tăng ca";
                worksheet.Cells[1, 6].Value = "Lương tăng ca";
                worksheet.Cells[1, 7].Value = "Ngày làm thêm";
                worksheet.Cells[1, 8].Value = "Lương làm thêm";
                worksheet.Cells[1, 9].Value = "Phụ cấp xác định";
                worksheet.Cells[1, 10].Value = "Phụ cấp khác";
                worksheet.Cells[1, 11].Value = "Thưởng";
                worksheet.Cells[1, 12].Value = "Phụ cấp lễ tết";
                worksheet.Cells[1, 13].Value = "Tổng lương";
                worksheet.Cells[1, 14].Value = "Hoa hồng";
                worksheet.Cells[1, 15].Value = "Phạt";
                worksheet.Cells[1, 16].Value = "Tạm ứng";
                worksheet.Cells[1, 17].Value = "Thực lĩnh";
                worksheet.Cells[1, 18].Value = "Phiếu chi";
                worksheet.Cells["A1:R1"].Style.Font.Bold = true;

                var row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.Employee.Name;
                    worksheet.Cells[row, 2].Value = item.DaySalary;
                    worksheet.Cells[row, 3].Value = item.WorkedDay;
                    worksheet.Cells[row, 4].Value = item.TotalBasicSalary.GetValueOrDefault().ToString("N1", CultureInfo.InvariantCulture);
                    worksheet.Cells[row, 5].Value = item.OverTimeHour;
                    worksheet.Cells[row, 6].Value = item.OverTimeHourSalary.GetValueOrDefault().ToString("N1", CultureInfo.InvariantCulture);
                    worksheet.Cells[row, 7].Value = item.OverTimeDay;
                    worksheet.Cells[row, 8].Value = item.OverTimeDaySalary.GetValueOrDefault().ToString("N1", CultureInfo.InvariantCulture);
                    worksheet.Cells[row, 9].Value = item.Employee.Allowance.GetValueOrDefault().ToString("N1", CultureInfo.InvariantCulture);
                    worksheet.Cells[row, 10].Value = item.OtherAllowance.GetValueOrDefault().ToString("N1", CultureInfo.InvariantCulture);
                    worksheet.Cells[row, 11].Value = item.RewardSalary.GetValueOrDefault().ToString("N1", CultureInfo.InvariantCulture);
                    worksheet.Cells[row, 12].Value = item.HolidayAllowance.GetValueOrDefault().ToString("N1", CultureInfo.InvariantCulture);
                    worksheet.Cells[row, 13].Value = item.TotalSalary.GetValueOrDefault().ToString("N1", CultureInfo.InvariantCulture);
                    worksheet.Cells[row, 14].Value = item.CommissionSalary.GetValueOrDefault().ToString("N1", CultureInfo.InvariantCulture);
                    worksheet.Cells[row, 15].Value = item.AmercementMoney.GetValueOrDefault().ToString("N1", CultureInfo.InvariantCulture);
                    worksheet.Cells[row, 16].Value = item.AdvancePayment.GetValueOrDefault().ToString("N1", CultureInfo.InvariantCulture); ;
                    worksheet.Cells[row, 17].Value = item.NetSalary.GetValueOrDefault().ToString("N1", CultureInfo.InvariantCulture);
                    worksheet.Cells[row, 18].Value = item.SalaryPayment !=null ? item.SalaryPayment.Name : "Chưa chi";

                    //format
                    //worksheet.Cells[row, 2].Style.Numberformat.Format = "#,#";
                    //worksheet.Cells[row, 4].Style.Numberformat.Format = "#,#";
                    //worksheet.Cells[row, 6].Style.Numberformat.Format = "#,#";
                    //worksheet.Cells[row, 8].Style.Numberformat.Format = "#,#";
                    //worksheet.Cells[row, 9].Style.Numberformat.Format = "#,#";
                    //worksheet.Cells[row, 10].Style.Numberformat.Format = "#,#";
                    //worksheet.Cells[row, 11].Style.Numberformat.Format = "#,#";
                    //worksheet.Cells[row, 12].Style.Numberformat.Format = "#,#";
                    //worksheet.Cells[row, 13].Style.Numberformat.Format = "#,#";
                    //worksheet.Cells[row, 14].Style.Numberformat.Format = "#,#";
                    //worksheet.Cells[row, 15].Style.Numberformat.Format = "#,#";
                    //worksheet.Cells[row, 17].Style.Numberformat.Format = "#,#";

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