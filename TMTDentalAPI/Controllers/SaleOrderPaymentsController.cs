﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleOrderPaymentsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly ISaleOrderPaymentService _saleOrderPaymentService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IViewRenderService _viewRenderService;
        private readonly IPrintTemplateConfigService _printTemplateConfigService;

        public SaleOrderPaymentsController(IMapper mapper, ISaleOrderPaymentService saleOrderPaymentService, IUnitOfWorkAsync unitOfWork, IViewRenderService viewRenderService,
            IPrintTemplateConfigService printTemplateConfigService
            )
        {
            _mapper = mapper;
            _saleOrderPaymentService = saleOrderPaymentService;
            _unitOfWork = unitOfWork;
            _viewRenderService = viewRenderService;
            _printTemplateConfigService = printTemplateConfigService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SaleOrderPaymentPaged val)
        {
            var result = await _saleOrderPaymentService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetHistoryPartnerAdvance([FromQuery] HistoryPartnerAdvanceFilter val)
        {
            var result = await _saleOrderPaymentService.GetPagedResultHistoryAdvanceAsync(val);
            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();

            var saleOrderPayment = _saleOrderPaymentService.GetDisplay(id);

            _unitOfWork.Commit();

            return Ok(saleOrderPayment);
        }

        [HttpPost]
        [CheckAccess(Actions = "Basic.SaleOrderPayment.Full")]
        public async Task<IActionResult> Create(SaleOrderPaymentSave val)
        {
            var saleOrderPayment = await _saleOrderPaymentService.CreateSaleOrderPayment(val);
            var basic = _mapper.Map<SaleOrderPaymentBasic>(saleOrderPayment);
            return Ok(basic);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionPayment(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderPaymentService.ActionPayment(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.SaleOrderPayment.Full")]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderPaymentService.ActionCancel(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

       

        //[HttpGet("{id}/[action]")]
        //public async Task<IActionResult> GetPrint(Guid id)
        //{
        //    var res = await _saleOrderPaymentService.GetPrint(id);
        //    if (res == null) return NotFound();
        //    var html = await _printTemplateConfigService.PrintOfType(new PrintOfTypeReq() { Obj = res, Type = "tmp_account_payment" });

        //    return Ok(new PrintData() { html = html });
        //}
    }
}
