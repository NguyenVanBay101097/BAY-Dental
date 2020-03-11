using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountInvoicesController : BaseApiController
    {
        private readonly IAccountInvoiceService _accountInvoiceService;
        private readonly IDotKhamService _dotKhamService;
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IApplicationRoleFunctionService _roleFunctionService;
        private readonly IIRModelAccessService _modelAccessService;

        public AccountInvoicesController(IAccountInvoiceService accountInvoiceService, IMapper mapper,
            ICompanyService companyService, IDotKhamService dotKhamService,
            IUnitOfWorkAsync unitOfWork, IApplicationRoleFunctionService roleFunctionService,
            IIRModelAccessService modelAccessService)
        {
            _accountInvoiceService = accountInvoiceService;
            _mapper = mapper;
            _companyService = companyService;
            _unitOfWork = unitOfWork;
            _dotKhamService = dotKhamService;
            _roleFunctionService = roleFunctionService;
            _modelAccessService = modelAccessService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]AccountInvoicePaged val)
        {
            _modelAccessService.Check("AccountInvoice", "Read");
            var result = await _accountInvoiceService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            _modelAccessService.Check("AccountInvoice", "Read");
            var accountInvoice = await _accountInvoiceService.GetAccountInvoiceForDisplayAsync(id);
            if (accountInvoice == null)
            {
                return NotFound();
            }
            var res = _mapper.Map<AccountInvoiceDisplay>(accountInvoice);
            res.InvoiceLines = res.InvoiceLines.OrderBy(x => x.Sequence);
            foreach (var inl in res.InvoiceLines)
            {
                inl.Teeth = inl.Teeth.OrderBy(x => x.Name);
            }
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccountInvoiceDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            _modelAccessService.Check("AccountInvoice", "Create");
            var inv = _mapper.Map<AccountInvoice>(val);
            SaveInvoiceLines(val, inv);
            await _accountInvoiceService.CreateAsync(inv);

            val.Id = inv.Id;
            return Ok(val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, AccountInvoiceDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            _modelAccessService.Check("AccountInvoice", "Update");
            var inv = await _accountInvoiceService.GetAccountInvoiceForDisplayAsync(id);
            if (inv == null)
                return NotFound();

            inv = _mapper.Map(val, inv);

            SaveInvoiceLines(val, inv);

            await _accountInvoiceService.UpdateInvoice(inv);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            _modelAccessService.Check("AccountInvoice", "Unlink");



            await _accountInvoiceService.DeleteInvoice(id);

            return NoContent();
        }


        [HttpPost("DefaultGet")]
        public async Task<IActionResult> DefaultGet(AccountInvoiceDisplay val)
        {
            var res = await _accountInvoiceService.DefaultGet(val);
            return Ok(res);
        }

        [HttpPost("InvoiceOpen")]
        public async Task<IActionResult> InvoiceOpen(IEnumerable<Guid> ids)
        {
            if (ids == null || ids.Count() == 0)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _accountInvoiceService.ActionInvoiceOpen(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("ActionCancel")]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            if (ids == null || ids.Count() == 0)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _accountInvoiceService.ActionCancel(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("ActionCancelDraft")]
        public async Task<IActionResult> ActionCancelDraft(IEnumerable<Guid> ids)
        {
            if (ids == null || ids.Count() == 0)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _accountInvoiceService.ActionCancelDraft(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Unlink(IEnumerable<Guid> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _accountInvoiceService.Unlink(ids);
            _unitOfWork.Commit();
            return NoContent();
        }


        [HttpGet("{id}/GetPaymentInfoJson")]
        public async Task<IActionResult> GetPaymentInfoJson(Guid id)
        {
            var res = await _accountInvoiceService._GetPaymentInfoJson(id);
            return Ok(res);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetSaleOrderPaymentInfoJson(Guid id)
        {
            var res = await _accountInvoiceService._GetSaleOrderPaymentInfoJson(id);
            return Ok(res);
        }

        [HttpGet("{id}/GetDotKhamList")]
        public async Task<IActionResult> GetDotKhamList(Guid id)
        {
            var dotKhams = await _dotKhamService.GetDotKhamsForInvoice(id);
            var res = _mapper.Map<IEnumerable<DotKhamBasic>>(dotKhams);
            return Ok(res);
        }

        [HttpGet("{id}/GetDotKhamList2")]
        public async Task<IActionResult> GetDotKhamList2(Guid id)
        {
            var dotKhams = await _dotKhamService.GetDotKhamsForInvoice(id);
            var res = _mapper.Map<IEnumerable<DotKhamDisplay>>(dotKhams);
            return Ok(res);
        }

        //Trả về danh sách những hóa đơn trạng thái khác draft để sử dụng cho combobox chọn hóa đơn điều trị
        [HttpGet("GetOpenPaid")]
        public async Task<IActionResult> GetOpenPaid(string search = "")
        {
            var res = await _accountInvoiceService.GetOpenPaid(search: search);
            return Ok(res);
        }

        [HttpGet("{id}/Print")]
        public async Task<IActionResult> GetPrint(Guid id)
        {
            var res = await _accountInvoiceService.GetAccountInvoicePrint(id);
            res.InvoiceLines = res.InvoiceLines.OrderBy(x => x.Sequence);
            return Ok(res);
        }

        private void SaveInvoiceLines(AccountInvoiceDisplay val, AccountInvoice order)
        {
            var existLines = order.InvoiceLines.ToList();
            var lineToRemoves = new List<AccountInvoiceLine>();
            foreach (var existLine in existLines)
            {
                bool found = false;
                foreach (var item in val.InvoiceLines)
                {
                    if (item.Id == existLine.Id)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                order.InvoiceLines.Remove(line);
            }

            //Cập nhật sequence cho tất cả các line của val
            int sequence = 1;
            foreach (var line in val.InvoiceLines)
            {
                line.Sequence = sequence++;
            }

            foreach (var line in val.InvoiceLines)
            {
                if (line.Id == Guid.Empty)
                {
                    var invL = _mapper.Map<AccountInvoiceLine>(line);
                    foreach (var tooth in line.Teeth)
                    {
                        invL.AccountInvoiceLineToothRels.Add(new AccountInvoiceLineToothRel
                        {
                            ToothId = tooth.Id
                        });
                    }
                    order.InvoiceLines.Add(invL);
                }
                else
                {
                    var invL = order.InvoiceLines.SingleOrDefault(c => c.Id == line.Id);
                    if (invL != null)
                    {
                        _mapper.Map(line, invL);
                        invL.AccountInvoiceLineToothRels.Clear();
                        foreach (var tooth in line.Teeth)
                        {
                            invL.AccountInvoiceLineToothRels.Add(new AccountInvoiceLineToothRel
                            {
                                ToothId = tooth.Id
                            });
                        }
                    }
                }
            }
        }
    }
}