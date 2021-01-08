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
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LaboOrdersController : BaseApiController
    {
        private readonly ILaboOrderService _laboOrderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDotKhamService _dotKhamService;
        private readonly IViewRenderService _viewRenderService;

        public LaboOrdersController(ILaboOrderService laboOrderService, IMapper mapper,
            IUnitOfWorkAsync unitOfWork, IDotKhamService dotKhamService, IViewRenderService viewRenderService)
        {
            _laboOrderService = laboOrderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _dotKhamService = dotKhamService;
            _viewRenderService = viewRenderService;
        }

        [HttpGet]
        [CheckAccess(Actions = "Labo.LaboOrder.Read")]
        public async Task<IActionResult> Get([FromQuery]LaboOrderPaged val)
        {
            var result = await _laboOrderService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Labo.LaboOrder.Read")]
        public async Task<IActionResult> GetFromSaleOrder_OrderLine([FromQuery] LaboOrderPaged val)
        {
            var res = await _laboOrderService.GetFromSaleOrder_OrderLine(val);
            return Ok(res);
        }   

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Labo.LaboOrder.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _laboOrderService.GetLaboDisplay(id);
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Labo.LaboOrder.Create")]
        public async Task<IActionResult> Create(LaboOrderSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var labo = await _laboOrderService.CreateLabo(val);
            _unitOfWork.Commit();
            return Ok(_mapper.Map<LaboOrderDisplay>(labo));
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Labo.LaboOrder.Update")]
        public async Task<IActionResult> Update(Guid id, LaboOrderSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
        
            await _unitOfWork.BeginTransactionAsync();
            await _laboOrderService.UpdateLabo(id, val);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Labo.LaboOrder.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _laboOrderService.Unlink(new List<Guid>() { id });
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> DefaultGet(LaboOrderDefaultGet val)
        {
            var res = await _laboOrderService.DefaultGet(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Labo.LaboOrder.Update")]
        public async Task<IActionResult> ButtonConfirm(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _laboOrderService.ButtonConfirm(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Labo.LaboOrder.Cancel")]
        public async Task<IActionResult> ButtonCancel(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _laboOrderService.ButtonCancel(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Labo.LaboOrder.Cancel")]
        public async Task<IActionResult> ActionCancelReceipt(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _laboOrderService.ActionCancelReceipt(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Labo.LaboOrder.Delete")]
        public async Task<IActionResult> Unlink(IEnumerable<Guid> ids)
        {
            if (ids == null || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _laboOrderService.Unlink(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpGet("{id}/[action]")]
        [CheckAccess(Actions = "Labo.LaboOrder.Read")]
        public async Task<IActionResult> GetPrint(Guid id)
        {
            var order = await _laboOrderService.SearchQuery(x => x.Id == id)
                .Include(x => x.Company.Partner)
                .Include(x => x.Product)
                .Include(x => x.LaboBridge)
                .Include(x => x.LaboBiteJoint)
                .Include(x => x.LaboFinishLine)
                .Include(x => x.SaleOrderLine.Product)
                .Include(x => x.SaleOrderLine.Order)
                .Include(x => x.SaleOrderLine.Employee)
                .Include(x => x.LaboOrderProductRel).ThenInclude(x => x.Product)
                .Include(x => x.Partner)
                .Include(x => x.Customer)
                .Include("LaboOrderToothRel.Tooth")
                .FirstOrDefaultAsync();

            if (order == null)
                return NotFound();

            var html = _viewRenderService.Render("LaboOrder/Print", order);

            return Ok(new PrintData() { html = html });
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Labo.LaboOrder.Read")] 
        public async Task<IActionResult> Statistics(LaboOrderStatisticsPaged val)
        {
            var result = await _laboOrderService.GetStatisticsPaged(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Labo.OrderLabo.Read")]
        public async Task<IActionResult> LaboOrderReport(LaboOrderReportInput val)
        {
            var result = await _laboOrderService.GetLaboOrderReport(val);
            return Ok(result);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Labo.LaboOrder.Read")]
        public async Task<IActionResult> GetLaboForSaleOrderLine([FromQuery] LaboOrderPaged val)
        {
            var res = await _laboOrderService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Labo.OrderLabo.Read")]
        public async Task<IActionResult> GetOrderLabo([FromQuery] OrderLaboPaged val)
        {

            var res = await _laboOrderService.GetPagedOrderLaboAsync(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Labo.ExportLabo.Read")]
        public async Task<IActionResult> GetExportLabo([FromQuery] ExportLaboPaged val)
        {
            var res = await _laboOrderService.GetPagedExportLaboAsync(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Labo.OrderLabo.Update")]
        public async Task<IActionResult> UpdateOrderLabo(LaboOrderReceiptSave val)
        {
            var labo = await _laboOrderService.GetByIdAsync(val.Id);
            labo.DateReceipt = val.DateReceipt;
            labo.WarrantyCode = val.WarrantyCode;
            labo.WarrantyPeriod = val.WarrantyPeriod;
            await _laboOrderService.UpdateAsync(labo);
            return NoContent();

        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Labo.ExportLabo.Update")]
        public async Task<IActionResult> UpdateExportLabo(ExportLaboOrderSave val)
        {
            var labo = await _laboOrderService.GetByIdAsync(val.Id);
            labo.DateExport = val.DateExport.HasValue ? val.DateExport : null;
            await _laboOrderService.UpdateAsync(labo);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CheckExistWarrantyCode(LaboOrderCheck val)
        {
            var res = await _laboOrderService.CheckExistWarrantyCode(val);
            return Ok(res);
        }
    }
}