﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Services;
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
    public class CashBooksController : ControllerBase
    {
        private readonly ICashBookService _cashBookService;
        public CashBooksController(ICashBookService cashBookService)
        {
            _cashBookService = cashBookService;
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Account.Read")]
        public async Task<IActionResult> GetSumary(CashBookSearch val)
        {
            var res = await _cashBookService.GetSumary(val.DateFrom, val.DateTo, val.CompanyId, val.ResultSelection,val.JournalId, val.AccountCode, val.PaymentType);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Account.Read")]
        public async Task<IActionResult> GetSumaryDayReport(CashBookSearch val)
        {
            var res = await _cashBookService.GetSumaryDayReport(val.DateFrom, val.DateTo, val.CompanyId, val.ResultSelection, val.JournalId);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetSumaryCashBookReport(SumaryCashBookFilter val)
        {
            var res = await _cashBookService.GetSumaryCashBookReport(val.DateFrom, val.DateTo, val.CompanyId, val.PartnerType, val.AccountCode, val.ResultSelection);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Account.Read")]
        public async Task<IActionResult> GetDetails(CashBookDetailFilter val)
        {
            var res = await _cashBookService.GetDetails(val.DateFrom, val.DateTo, val.Limit, val.Offset, val.CompanyId, val.Search, val.ResultSelection , val.JournalId, accountIds: val.AccountIds, paymentType: val.PaymentType);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Account.Read")]
        public async Task<IActionResult> GetDataInvoices(DataInvoiceFilter val)
        {
            var res = await _cashBookService.GetDataInvoices(val.DateFrom, val.DateTo, val.CompanyId, val.ResultSelection);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Account.Read")]
        public async Task<IActionResult> GetChartReport(CashBookReportFilter val)
        {
            var res = await _cashBookService.GetChartReport(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Account.Read")]
        public async Task<IActionResult> GetTotal(CashBookSearch val)
        {
            var res = await _cashBookService.GetTotal(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Account.Export")]
        public async Task<IActionResult> ExportExcelFile(CashBookDetailFilter val)
        {
            var stream = new MemoryStream();
            val.Limit = int.MaxValue;
            val.Offset = 0;
            var services = await _cashBookService.GetDetails(val.DateFrom, val.DateTo, val.Limit, val.Offset, val.CompanyId.Value, val.Search, val.ResultSelection , val.JournalId, val.AccountIds, val.PaymentType);
            var sheetName = "Tổng sổ quỹ";
            if (val.ResultSelection == "cash")
            {
                sheetName = "Sổ quỹ tiền mặt";
            }
            else if (val.ResultSelection == "bank")
            {
                sheetName = "Sổ quỹ ngân hàng";
            }

            byte[] fileContent;

            var items = services.Items.ToList();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                worksheet.Cells[1, 1].Value = "Ngày";
                worksheet.Cells[1, 2].Value = "Diễn giải";
                worksheet.Cells[1, 3].Value = "Phương thức";
                worksheet.Cells[1, 4].Value = "Loại thu chi";
                worksheet.Cells[1, 5].Value = "Số tiền";
                worksheet.Cells[1, 6].Value = "Đối tác";
                for (int row = 2; row < items.Count + 2; row++)
                {
                    var item = items[row - 2];

                    worksheet.Cells[row, 1].Value = item.Date;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "d/m/yyyy";
                    worksheet.Cells[row, 2].Value = item.Name;
                    worksheet.Cells[row, 3].Value = item.JournalName;
                    worksheet.Cells[row, 4].Value = item.AccountName;
                    worksheet.Cells[row, 5].Value = item.Amount;
                    worksheet.Cells[row, 6].Value = item.PartnerName;
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
