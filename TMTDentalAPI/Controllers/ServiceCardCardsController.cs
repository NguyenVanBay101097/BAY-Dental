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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceCardCardsController : BaseApiController
    {
        private readonly IServiceCardCardService _cardCardService;
        private readonly ISaleOrderServiceCardCardRelService _orderCardRelService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public ServiceCardCardsController(IServiceCardCardService cardCardService,
            ISaleOrderServiceCardCardRelService orderCardRelService, IUnitOfWorkAsync unitOfWork,
            IMapper mapper)
        {
            _cardCardService = cardCardService;
            _orderCardRelService = orderCardRelService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [CheckAccess(Actions = "ServiceCard.Card.Read")]
        public async Task<IActionResult> Get([FromQuery] ServiceCardCardPaged val)
        {
            var res = await _cardCardService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ServiceCardCardSave val)
        {
            var entity = _mapper.Map<ServiceCardCard>(val);
            await _unitOfWork.BeginTransactionAsync();
            await _cardCardService.CreateAsync(entity);
            _unitOfWork.Commit();

            var basic = _mapper.Map<ServiceCardCardDisplay>(entity);
            return Ok(basic);
        }
        [HttpPut("{id}")]
        [CheckAccess(Actions = "ServiceCard.Type.Update")]
        public async Task<IActionResult> Update(Guid id, ServiceCardCardSave val)
        {
            var entity = await _cardCardService.GetByIdAsync(id);
            if (entity == null)
                return NotFound();

            await _unitOfWork.BeginTransactionAsync();
            entity = _mapper.Map(val, entity);
            await _cardCardService.UpdateAsync(entity);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _cardCardService.Unlink(new List<Guid>() { id });

            return NoContent();
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetHistories(Guid id)
        {
            var rels = await _orderCardRelService.SearchQuery(x => x.CardId == id, orderBy: s => s.OrderByDescending(m => m.DateCreated))
                .Include(x => x.Card)
                .Include(x => x.SaleOrder).ToListAsync();
            return Ok(_mapper.Map<IEnumerable<SaleOrderServiceCardCardRelBasic>>(rels));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetServiceCardCards(ServiceCardCardFilter val)
        {
            var rels = await _cardCardService.GetServiceCardCards(val);
            return Ok(rels);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "ServiceCard.Card.Update")]
        public async Task<IActionResult> ButtonActive(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _cardCardService.ActionActive(ids);
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "ServiceCard.Card.Update")]
        public async Task<IActionResult> ButtonLock(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _cardCardService.ActionLock(ids);
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "ServiceCard.Card.Update")]
        public async Task<IActionResult> ButtonCancel(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _cardCardService.ActionCancel(ids);
            return NoContent();
        }

        //[HttpPost("[action]")]
        //[CheckAccess(Actions = "ServiceCard.Card.Read")]
        //public async Task<IActionResult> ExportExcel(ServiceCardCardPaged val)
        //{
        //    var stream = new MemoryStream();
        //    var paged = await _cardCardService.GetPagedResultAsync(val);
        //    var sheetName = "Thẻ dịch vụ";
        //    byte[] fileContent;

        //    using (var package = new ExcelPackage(stream))
        //    {
        //        var worksheet = package.Workbook.Worksheets.Add(sheetName);

        //        worksheet.Cells[1, 1].Value = "Số thẻ";
        //        worksheet.Cells[1, 2].Value = "Mã vạch";
        //        worksheet.Cells[1, 3].Value = "Loại thẻ";
        //        worksheet.Cells[1, 4].Value = "Ngày cấp";
        //        worksheet.Cells[1, 5].Value = "Ngày hết hạn";

        //        var row = 2;
        //        foreach (var item in paged.Items)
        //        {
        //            worksheet.Cells[row, 1].Value = item.Name;
        //            worksheet.Cells[row, 2].Value = item.Barcode;
        //            worksheet.Cells[row, 3].Value = item.CardTypeName;
        //            worksheet.Cells[row, 4].Value = item.ActivatedDate;
        //            worksheet.Cells[row, 4].Style.Numberformat.Format = "dd-mm-yyyy";

        //            worksheet.Cells[row, 5].Value = item.ExpiredDate;
        //            worksheet.Cells[row, 5].Style.Numberformat.Format = "dd-mm-yyyy";

        //            row++;
        //        }

        //        worksheet.Cells.AutoFitColumns();

        //        package.Save();

        //        fileContent = stream.ToArray();
        //    }

        //    string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        //    return new FileContentResult(fileContent, mimeType);
        //}

        [HttpGet("[action]")]
        public async Task<IActionResult> CheckCode(string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest();

            var card = await _cardCardService.CheckCode(code);
            var res = _mapper.Map<ServiceCardCardBasic>(card);
            return Ok(res);
        }
    }
}