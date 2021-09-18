﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToaThuocsController : BaseApiController
    {
        private readonly IToaThuocService _toaThuocService;
        private readonly IMapper _mapper;
        private readonly IIRModelAccessService _modelAccessService;
        private readonly IViewRenderService _view;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IPrintTemplateConfigService _printTemplateConfigService;
        private readonly IPrintTemplateService _printTemplateService;
        private readonly IIRModelDataService _modelDataService;

        public ToaThuocsController(IToaThuocService toaThuocService, IMapper mapper, IViewRenderService view,
            IIRModelAccessService modelAccessService, IUnitOfWorkAsync unitOfWork, IPrintTemplateConfigService printTemplateConfigService, IPrintTemplateService printTemplateService,
            IIRModelDataService modelDataService)
        {
            _toaThuocService = toaThuocService;
            _mapper = mapper;
            _modelAccessService = modelAccessService;
            _view = view;
            _unitOfWork = unitOfWork;
            _printTemplateConfigService = printTemplateConfigService;
            _printTemplateService = printTemplateService;
            _modelDataService = modelDataService;
        }

        [HttpGet]
        [CheckAccess(Actions = "Medicine.ToaThuoc.Read")]
        public async Task<IActionResult> Get([FromQuery] ToaThuocPaged val)
        {
            var result = await _toaThuocService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Medicine.ToaThuoc.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var toaThuoc = await _toaThuocService.GetToaThuocForDisplayAsync(id);
            if (toaThuoc == null)
                return NotFound();

            return Ok(toaThuoc);
        }

        [HttpPost]
        [CheckAccess(Actions = "Medicine.ToaThuoc.Create")]
        public async Task<IActionResult> Create(ToaThuocSave val)
        {
            var order = _mapper.Map<ToaThuoc>(val);
            SaveOrderLines(val, order);
            await _toaThuocService.CreateAsync(order);

            var Medicine = _mapper.Map<ToaThuocBasic>(order);
            return Ok(Medicine);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Medicine.ToaThuoc.Update")]
        public async Task<IActionResult> Update(Guid id, ToaThuocSave val)
        {
            var toathuoc = await _toaThuocService.SearchQuery(x => x.Id == id).Include(x => x.Lines).FirstOrDefaultAsync();
            if (toathuoc == null)
                return NotFound();

            toathuoc = _mapper.Map(val, toathuoc);

            SaveOrderLines(val, toathuoc);

            await _toaThuocService.Write(toathuoc);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Medicine.ToaThuoc.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var toaThuoc = await _toaThuocService.GetByIdAsync(id);
            if (toaThuoc == null)
                return NotFound();
            await _toaThuocService.DeleteAsync(toaThuoc);

            return NoContent();
        }

        [HttpPost("DefaultGet")]
        public async Task<IActionResult> DefaultGet(ToaThuocDefaultGet val)
        {
            var res = await _toaThuocService.DefaultGet(val);
            return Ok(res);
        }

        [HttpPost("LineDefaultGet")]
        public async Task<IActionResult> LineDefaultGet(ToaThuocLineDefaultGet val)
        {
            var res = await _toaThuocService.LineDefaultGet(val);
            return Ok(res);
        }

        [HttpGet("{id}/Print")]
        [CheckAccess(Actions = "Medicine.ToaThuoc.Read")]
        public async Task<IActionResult> GetPrint(Guid id)
        {
            //tim trong bảng config xem có dòng nào để lấy ra template
            var printConfig = await _printTemplateConfigService.SearchQuery(x => x.Type == "tmp_toathuoc" && x.IsDefault)
                .Include(x => x.PrintPaperSize)
                .Include(x => x.PrintTemplate)
                .FirstOrDefaultAsync();

            PrintTemplate template = printConfig != null ? printConfig.PrintTemplate : null;
            PrintPaperSize paperSize = printConfig != null ? printConfig.PrintPaperSize : null;
            if (template == null)
            {
                //tìm template mặc định sử dụng chung cho tất cả chi nhánh, sử dụng bảng IRModelData hoặc bảng IRConfigParameter
                template = await _modelDataService.GetRef<PrintTemplate>("base.print_template_toa_thuoc");
                if (template == null)
                    throw new Exception("Không tìm thấy mẫu in mặc định");
            }

            var result = await _printTemplateService.GeneratePrintHtml(template, id, paperSize);
            return Ok(new PrintData() { html = result });
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetFromUI(Guid id)
        {
            var toaThuoc = await _toaThuocService.GetToaThuocFromUIAsync(id);
            if (toaThuoc == null)
                return NotFound();

            return Ok(toaThuoc);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateFromUI(ToaThuocSaveFromUI val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var result = await _toaThuocService.CreateToaThuocFromUIAsync(val);
            _unitOfWork.Commit();
            return Ok(result);
        }

        [HttpPut("{id}/[action]")]
        public async Task<IActionResult> UpdateFromUI(Guid id, ToaThuocSaveFromUI val)
        {
            await _toaThuocService.UpdateToaThuocFromUIAsync(id, val);

            return NoContent();
        }

        private void SaveOrderLines(ToaThuocSave val, ToaThuoc order)
        {
            var lineToRemoves = new List<ToaThuocLine>();

            foreach (var existLine in order.Lines)
            {
                if (!val.Lines.Any(x => x.Id == existLine.Id))
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                order.Lines.Remove(line);
            }

            int sequence = 1;

            foreach (var line in val.Lines)
            {
                if (line.Id == Guid.Empty)
                {
                    var l = _mapper.Map<ToaThuocLine>(line);
                    l.Sequence = sequence;
                    order.Lines.Add(l);
                }
                else
                {
                    var l = order.Lines.SingleOrDefault(c => c.Id == line.Id);
                    _mapper.Map(line, l);
                    l.Sequence = sequence;
                }

                sequence++;
            }
        }
    }
}