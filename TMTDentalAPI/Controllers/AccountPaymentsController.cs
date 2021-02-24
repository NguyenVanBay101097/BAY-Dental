using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
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
    public class AccountPaymentsController : BaseApiController
    {
        private readonly IAccountPaymentService _paymentService;
        private readonly IViewRenderService _viewRenderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IAccountMoveService _accountMoveService;
        private readonly IPartnerService _partnerService;

        public AccountPaymentsController(IAccountPaymentService paymentService, IViewRenderService viewRenderService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork, IAccountMoveService accountMoveService,
            IPartnerService partnerService)
        {
            _paymentService = paymentService;
            _viewRenderService = viewRenderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _accountMoveService = accountMoveService;
            _partnerService = partnerService;
        }

        [HttpGet]
        [CheckAccess(Actions = "Basic.AccountPayment.Read")]
        public async Task<IActionResult> Get([FromQuery] AccountPaymentPaged val)
        {
            var result = await _paymentService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Basic.AccountPayment.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var payment = await _paymentService.SearchQuery(x => x.Id == id)
                .Include(x => x.Partner)
                .Include(x => x.Journal)
                .Include(x => x.LoaiThuChi)
                .FirstOrDefaultAsync();
            if (payment == null)
            {
                return NotFound();
            }

            var res = _mapper.Map<AccountPaymentDisplay>(payment);
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Basic.AccountPayment.Create")]
        public async Task<IActionResult> Create(AccountPaymentSave val)
        {
            var payment = await _paymentService.CreateUI(val);
            var basic = _mapper.Map<AccountPaymentBasic>(payment);
            return Ok(basic);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.AccountPayment.Create")]
        public async Task<IActionResult> CreateMultipleAndConfirmUI(List<AccountPaymentSave> vals)
        {
            var payment = await _paymentService.CreateMultipleAndConfirmUI(vals);
            var basic = _mapper.Map<IEnumerable<AccountPaymentBasic>>(payment);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Basic.AccountPayment.update")]
        public async Task<IActionResult> Update(Guid id, AccountPaymentSave val)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment == null)
                return NotFound();
            payment = _mapper.Map(val, payment);
            await _paymentService.UpdateAsync(payment);

            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.AccountPayment.Update")]
        public async Task<IActionResult> Post(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _paymentService.Post(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.AccountPayment.Update")]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _paymentService.CancelAsync(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.AccountPayment.Delete")]
        public async Task<IActionResult> Unlink(IEnumerable<Guid> ids)
        {
            //Chỉ xóa account payment ở trạng thái nháp hoặc cancel
            //Muốn xóa thì phải hủy trước
            if (ids == null || !ids.Any())
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _paymentService.UnlinkAsync(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SaleDefaultGet(IEnumerable<Guid> ids)
        {
            var res = await _paymentService.SaleDefaultGet(ids);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> PurchaseDefaultGet(IEnumerable<Guid> ids)
        {
            var res = await _paymentService.PurchaseDefaultGet(ids);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ServiceCardOrderDefaultGet(IEnumerable<Guid> ids)
        {
            var res = await _paymentService.ServiceCardOrderDefaultGet(ids);
            return Ok(res);
        }

        [HttpGet("GetPaymentBasicList")]
        public async Task<IActionResult> GetPaymentBasicList([FromQuery] AccountPaymentFilter val)
        {
            var res = await _paymentService.GetPaymentBasicList(val);
            return Ok(res);
        }

        [HttpGet("{id}/[action]")]
        [CheckAccess(Actions = "Basic.AccountPayment.Read")]
        public async Task<IActionResult> GetPrint(Guid id)
        {
            var res = await _paymentService.GetPrint(id);
            return Ok(res);
        }

        //get default thanh toán cho nhà cung cấp
        [HttpPost("[action]")]
        public IActionResult SupplierDefaultGet(AccountPaymentSupplierDefaultGetRequest val)
        {
            //nếu list invoices ko có gì thì lấy công nợ hiện tại, else sum công nợ của invoice
            decimal? amountTotal = 0;
            if (val.InvoiceIds.Any())
            {
                amountTotal = _accountMoveService.SearchQuery(x => val.InvoiceIds.Contains(x.Id)).Sum(x => x.AmountResidualSigned);
            }
            else
            {
                var creditDebitGet = _partnerService.CreditDebitGet(new List<Guid>() { val.PartnerId });
                amountTotal = -creditDebitGet[val.PartnerId].Debit;
            }

            var result = new AccountRegisterPaymentDisplay
            {
                Amount = Math.Abs(amountTotal ?? 0),
                PaymentType = (amountTotal ?? 0) < 0 ? "outbound" : "inbound",
                PartnerId = val.PartnerId,
                PartnerType = "supplier",
                InvoiceIds = val.InvoiceIds
            };

            return Ok(result);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Account.Read")]
        public async Task<IActionResult> ExportExcelFile( AccountPaymentPaged val)
        {
            var stream = new MemoryStream();
            val.Limit = int.MaxValue;
            val.Offset = 0;
            var data = await _paymentService.GetPagedResultAsync(val);
            var sheetName = "Tổng sổ quỹ";
            if (val.JournalType == "cash")
            {
                sheetName = "Sổ quỹ tiền mặt";
            }
            else if (val.JournalType == "bank")
            {
                sheetName = "Sổ quỹ ngân hàng";
            }

            byte[] fileContent;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);
                var row = 2;
                if (val.JournalType == "cash_bank")
                {
                    worksheet.Cells[1, 1].Value = "Ngày";
                    worksheet.Cells[1, 2].Value = "Số phiếu";
                    worksheet.Cells[1, 3].Value = "Phương thức thanh toán";
                    worksheet.Cells[1, 4].Value = "Loại thu chi";
                    worksheet.Cells[1, 5].Value = "Số tiền";
                    worksheet.Cells[1, 6].Value = "Nhóm người nhận/nộp";
                    worksheet.Cells[1, 7].Value = "Người nhận/nộp tiền";

                    foreach (var item in data.Items)
                    {
                        worksheet.Cells[row, 1].Value = item.PaymentDate;
                        worksheet.Cells[row, 1].Style.Numberformat.Format = "d/m/yyyy";
                        worksheet.Cells[row, 2].Value = item.Name;
                        worksheet.Cells[row, 3].Value = item.JournalName;
                        worksheet.Cells[row, 4].Value = item.DestinationAccountName;
                        worksheet.Cells[row, 5].Value = (item.PaymentType == "outbound") ? -item.Amount : item.Amount;
                        worksheet.Cells[row, 6].Value = GetPartnerType(item.PartnerType);
                        worksheet.Cells[row, 7].Value = item.PartnerName;
                        row++;

                    }
                }
                else
                {
                    worksheet.Cells[1, 1].Value = "Ngày";
                    worksheet.Cells[1, 2].Value = "Số phiếu";
                    worksheet.Cells[1, 3].Value = "Loại thu chi";
                    worksheet.Cells[1, 4].Value = "Số tiền";
                    worksheet.Cells[1, 5].Value = "Nhóm người nhận/nộp";
                    worksheet.Cells[1, 6].Value = "Người nhận/nộp tiền";

                    foreach (var item in data.Items)
                    {
                        worksheet.Cells[row, 1].Value = item.PaymentDate;
                        worksheet.Cells[row, 1].Style.Numberformat.Format = "d/m/yyyy";
                        worksheet.Cells[row, 2].Value = item.Name;
                        worksheet.Cells[row, 3].Value = item.DestinationAccountName;
                        worksheet.Cells[row, 4].Value = (item.PaymentType == "outbound") ? -item.Amount : item.Amount;
                        worksheet.Cells[row, 5].Value = GetPartnerType(item.PartnerType);
                        worksheet.Cells[row, 6].Value = item.PartnerName;
                        row++;
                    }
                }

                worksheet.Cells.AutoFitColumns();

                package.Save();

                fileContent = stream.ToArray();
            }

            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            stream.Position = 0;

            return new FileContentResult(fileContent, mimeType);
        }

        private string GetPartnerType(string partnerType)
        {
            if (partnerType == "employee")
            {
                return "Nhân viên";
            }
            else if (partnerType == "customer")
            {
                return "Khách hàng";
            }
            else if (partnerType == "supplier")
            {
                return "Nhà cung cấp";
            }
            return "";

        }
    }
}