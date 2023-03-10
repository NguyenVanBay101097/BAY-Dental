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
using OfficeOpenXml.Style;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardCardsController : BaseApiController
    {
        private readonly ICardCardService _cardCardService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public CardCardsController(ICardCardService cardCardService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _cardCardService = cardCardService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [CheckAccess(Actions = "Card.Card.Read")]
        public async Task<IActionResult> Get([FromQuery] CardCardPaged val)
        {
            var res = await _cardCardService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Card.Card.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var card = await _cardCardService.SearchQuery(x => x.Id == id).Include(x => x.Type)
                .Include(x => x.Partner).FirstOrDefaultAsync();
            if (card == null)
                return NotFound();

            var display = _mapper.Map<CardCardDisplay>(card);
            display.IsExpired = display.ExpiredDate.HasValue ? DateTime.Today > display.ExpiredDate.Value : false;

            return Ok(display);
        }


        [HttpPost]
        [CheckAccess(Actions = "Card.Card.Create")]
        public async Task<IActionResult> Create(CardCardSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            var entity = _mapper.Map<CardCard>(val);
            entity.CompanyId = CompanyId;
            await _unitOfWork.BeginTransactionAsync();
            await _cardCardService.CreateAsync(entity);
            _unitOfWork.Commit();

            var basic = _mapper.Map<CardCardDisplay>(entity);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Card.Card.Update")]
        public async Task<IActionResult> Update(Guid id, CardCardSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var entity = await _cardCardService.GetByIdAsync(id);
            if (entity == null)
                return NotFound();

            await _unitOfWork.BeginTransactionAsync();
            entity = _mapper.Map(val, entity);
            if (!entity.CompanyId.HasValue)
                entity.CompanyId = CompanyId;
            await _cardCardService.UpdateAsync(entity);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Card.Card.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _cardCardService.Unlink(new List<Guid>() { id });
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Card.Card.Update")]
        public async Task<IActionResult> ButtonConfirm(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _cardCardService.ButtonConfirm(ids);
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Card.Card.Update")]
        public async Task<IActionResult> ButtonActive(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _cardCardService.ButtonActive(ids);
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Card.Card.Update")]
        public async Task<IActionResult> ButtonCancel(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _cardCardService.ButtonCancel(ids);
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Card.Card.Update")]
        public async Task<IActionResult> ButtonReset(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _cardCardService.ButtonReset(ids);
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Card.Card.Update")]
        public async Task<IActionResult> ButtonLock(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _cardCardService.ButtonLock(ids);
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Card.Card.Update")]
        public async Task<IActionResult> ButtonRenew(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _cardCardService.ButtonRenew(ids);
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Card.Card.Update")]
        public async Task<IActionResult> ButtonUnlock(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _cardCardService.ButtonUnlock(ids);
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Card.Card.Update")]
        public async Task<IActionResult> ButtonUpgradeCard(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _cardCardService.ButtonUpgradeCard(ids);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Card.Card.Read")]
        public async Task<IActionResult> ExportExcelFile([FromQuery] CardCardPaged val)
        {
            var stream = new MemoryStream();
            val.Limit = int.MaxValue;
            val.Offset = 0;
            var paged = await _cardCardService.GetPagedResultAsync(val);
            var sheetName = "Thông tin thẻ thành viên";
            byte[] fileContent;

            var stateDict = new Dictionary<string, string>()
            {
                {"draft","Chưa kích hoạt"},
                {"confirmed","Chờ cấp thẻ"},
                {"in_use","Đã kích hoạt"},
                {"locked","Đã khóa"},
                {"cancelled","Đã hủy"}
            };
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                worksheet.Cells[1, 1].Value = "Số ID thẻ";
                worksheet.Cells[1, 2].Value = "Hạng thẻ";
                worksheet.Cells[1, 3].Value = "Họ tên";
                worksheet.Cells[1, 4].Value = "Điện thoại";
                worksheet.Cells[1, 5].Value = "Trạng thái";

                worksheet.Cells["A1:E1"].Style.Font.Bold = true;
                worksheet.Cells["A1:E1"].Style.Font.Size = 12;
                worksheet.Cells["A1:E1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:E1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:E1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A1:E1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                var row = 2;
                foreach (var item in paged.Items)
                {
                    worksheet.Cells[row, 1].Value = item.Barcode;
                    worksheet.Cells[row, 2].Value = item.TypeName;
                    worksheet.Cells[row, 3].Value = item.PartnerName;
                    worksheet.Cells[row, 4].Value = item.PartnerPhone;
                    worksheet.Cells[row, 5].Value = stateDict.ContainsKey(item.State) ? stateDict[item.State] : "";

                    worksheet.Cells[row, 1, row, 5].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[row, 1, row, 5].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[row, 1, row, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[row, 1, row, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
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

        [HttpPost("ImportExcel")]
        [CheckAccess(Actions = "Card.Card.Read")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            var res = await _cardCardService.ActionImport(file);
            return Ok(res);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Card.Card.Read")]
        public async Task<IActionResult> GetDefault()
        {
            var res = await _cardCardService.GetDefault();
            var display = _mapper.Map<CardCardDisplay>(res);
            if (res.Type != null)
                display.TypeId = res.Type.Id;
            return Ok(display);
        }
    }
}