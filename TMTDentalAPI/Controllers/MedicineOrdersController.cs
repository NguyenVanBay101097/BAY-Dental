﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicineOrdersController : BaseApiController
    {
        private readonly IMedicineOrderService _medicineOrderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IViewRenderService _viewRenderService;
        private readonly IPrintTemplateConfigService _printTemplateConfigService;

        public MedicineOrdersController(IMedicineOrderService medicineOrderService, IMapper mapper, IUnitOfWorkAsync unitOfWork,
            IViewRenderService viewRenderService,
            IPrintTemplateConfigService printTemplateConfigService)
        {
            _medicineOrderService = medicineOrderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _viewRenderService = viewRenderService;
            _printTemplateConfigService = printTemplateConfigService;
        }

        [HttpGet]
        [CheckAccess(Actions = "Medicine.MedicineOrder.Read")]
        public async Task<IActionResult> Get([FromQuery] MedicineOrderPaged val)
        {
            var result = await _medicineOrderService.GetPagedResultAsync(val);
            return Ok(result);
        }


        ///Get{id}
        [HttpGet("{id}")]
        [CheckAccess(Actions = "Medicine.MedicineOrder.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _medicineOrderService.GetByIdDisplay(id);
            return Ok(res);
        }



        [HttpPost]
        public async Task<IActionResult> Create(MedicineOrderSave val)
        {

            await _unitOfWork.BeginTransactionAsync();

            var medicineOrder = await _medicineOrderService.CreateMedicineOrder(val);
            _unitOfWork.Commit();

            var display = _mapper.Map<MedicineOrderDisplay>(medicineOrder);
            return Ok(display);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, MedicineOrderSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _medicineOrderService.UpdateMedicineOrder(id, val);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Medicine.MedicineOrder.Read")]
        public async Task<IActionResult> DefaultGet(MedicineOrderDefaultGet val)
        {
            var result = await _medicineOrderService.DefaultGet(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Medicine.MedicineOrder.Payment")]
        public async Task<IActionResult> ActionPayment(MedicineOrderSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var rs = await _medicineOrderService.ActionPayment(val);
            _unitOfWork.Commit();
            return Ok(rs);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Medicine.MedicineOrder.Cancel")]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            if (ids == null || ids.Count() == 0)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _medicineOrderService.ActionCancel(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Medicine.MedicineOrder.Read")]
        public async Task<IActionResult> GetReport(MedicineOrderFilterReport val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var res = await _medicineOrderService.GetReport(val);
            _unitOfWork.Commit();
            return Ok(res);
        }

        //[HttpGet("{id}/[action]")]
        //[CheckAccess(Actions = "Medicine.MedicineOrder.Read")]
        //public async Task<IActionResult> GetPrint(Guid id)
        //{
        //    //get viewmodel và truyền vào view
        //    var res = await _medicineOrderService.GetPrint(id);
        //    var obj = _mapper.Map<MedicineOrderPrint>(res);
        //    var html = await _printTemplateConfigService.Print(obj , "tmp_medicine_order");

        //    return Ok(new PrintData() { html = html });
        //}
    }
}
