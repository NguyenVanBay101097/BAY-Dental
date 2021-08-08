﻿using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerReceiptsController : BaseApiController
    {
        private readonly ICustomerReceiptService _customerReceiptService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public CustomerReceiptsController(ICustomerReceiptService customerReceiptService, IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _customerReceiptService = customerReceiptService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;

        }


        [HttpGet]
        [CheckAccess(Actions = "Basic.CustomerReceipt.Read")]
        public async Task<IActionResult> Get([FromQuery] CustomerReceiptPaged val)
        {
            var result = await _customerReceiptService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Basic.CustomerReceipt.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _customerReceiptService.GetDisplayById(id);

            var display = _mapper.Map<CustomerReceiptDisplay>(res);

            return Ok(display);
        }

        [HttpPost]
        [CheckAccess(Actions = "Basic.CustomerReceipt.Create")]
        public async Task<IActionResult> Create(CustomerReceiptSave val)
        {
            if (!ModelState.IsValid || val == null)
                return BadRequest();
            var res = await _customerReceiptService.CreateCustomerReceipt(val);
            var basic = _mapper.Map<CustomerReceiptBasic>(res);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Basic.CustomerReceipt.Update")]
        public async Task<IActionResult> Update(Guid id, CustomerReceiptSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var entity = await _customerReceiptService.GetByIdAsync(id);
            if (entity == null)
                return NotFound();
            await _customerReceiptService.UpdateCustomerReceipt(id , val);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetCount(CustomerReceiptGetCountVM val)
        {
            var res = await _customerReceiptService.GetCountToday(val);
            return Ok(res);
        }

        [HttpPatch("{id}/[action]")]
        public async Task<IActionResult> PatchState(Guid id, CustomerReceiptStatePatch result)
        {
            var entity = await _customerReceiptService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            var patch = new JsonPatchDocument<CustomerReceiptStatePatch>();
            patch.Replace(x => x.State, result.State);
            patch.Replace(x => x.Reason, result.Reason);
            patch.Replace(x => x.IsNoTreatment, result.IsNoTreatment);
            patch.Replace(x => x.DateExamination, result.DateExamination);
            patch.Replace(x => x.DateDone, result.DateDone);
            var entityMap = _mapper.Map<CustomerReceiptStatePatch>(entity);
            patch.ApplyTo(entityMap);

            entity = _mapper.Map(entityMap, entity);
            await _customerReceiptService.UpdateAsync(entity);

            return NoContent();
        }
    }
}
