﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockPickingsController : BaseApiController
    {
        private readonly IStockPickingService _stockPickingService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IIRModelAccessService _modelAccessService;
        private readonly IStockMoveService _stockMoveService;

        public StockPickingsController(IStockPickingService stockPickingService, IMapper mapper,
            IUnitOfWorkAsync unitOfWork, IIRModelAccessService modelAccessService, IStockMoveService stockMoveService)
        {
            _stockPickingService = stockPickingService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _modelAccessService = modelAccessService;
            _stockMoveService = stockMoveService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]StockPickingPaged val)
        {
            var res = await _stockPickingService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var picking = await _stockPickingService.GetPickingForDisplay(id);
            if (picking == null)
                return NotFound();
            var res = _mapper.Map<StockPickingDisplay>(picking);
            res.MoveLines = res.MoveLines.OrderBy(x => x.Sequence);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(StockPickingDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
           
            var picking = _mapper.Map<StockPicking>(val);
            SaveMoveLines(val, picking);
            _stockMoveService._Compute(picking.MoveLines);
           
            await _unitOfWork.BeginTransactionAsync();
            await _stockPickingService.CreateAsync(picking);
            _unitOfWork.Commit();

            val.Id = picking.Id;
            return Ok(val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, StockPickingDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var picking = await _stockPickingService.GetPickingForDisplay(id);
            if (picking == null)
                return NotFound();

            picking = _mapper.Map(val, picking);
            SaveMoveLines(val, picking);

            _stockMoveService._Compute(picking.MoveLines);

            await _stockPickingService.UpdateAsync(picking);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _stockPickingService.Unlink(new List<Guid>() { id });
            _unitOfWork.Commit();
            return NoContent();
        }

        private void SaveMoveLines(StockPickingDisplay val, StockPicking picking)
        {
            if (picking.State != "draft")
                return;

            //remove line
            var lineToRemoves = new List<StockMove>();
            foreach (var existLine in picking.MoveLines)
            {
                if (!val.MoveLines.Any(x => x.Id == existLine.Id))
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                picking.MoveLines.Remove(line);
            }

            int sequence = 1;
            foreach (var line in val.MoveLines)
                line.Sequence = sequence++;

            foreach (var line in val.MoveLines)
            {
                if (line.Id == Guid.Empty)
                {
                    var ml = _mapper.Map<StockMove>(line);
                    ml.CompanyId = picking.CompanyId;
                    ml.LocationId = picking.LocationId;
                    ml.LocationDestId = picking.LocationDestId;
                    picking.MoveLines.Add(ml);
                }
                else
                {
                    _mapper.Map(line, picking.MoveLines.SingleOrDefault(c => c.Id == line.Id));
                }
            }
        }

        [HttpPost("DefaultGet")]
        public async Task<IActionResult> DefaultGet(StockPickingDefaultGet val)
        {
            var res = await _stockPickingService.DefaultGet(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> DefaultGetOutgoing()
        {
            var res = await _stockPickingService.DefaultGetOutgoing();
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> DefaultGetIncoming()
        {
            var res = await _stockPickingService.DefaultGetIncoming();
            return Ok(res);
        }

        [HttpPost("ActionDone")]
        public async Task<IActionResult> ActionDone(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _stockPickingService.ActionDone(ids);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("GetPaged")]
        public async Task<IActionResult> GetPaged(StockPickingPaged val)
        {
            var result = await _stockPickingService.GetPagedResultAsync(val);

            var paged = new PagedResult2<StockPickingBasic>(result.TotalItems, result.Offset, result.Limit)
            {
                //Có thể dùng thư viện automapper
                Items = _mapper.Map<IEnumerable<StockPickingBasic>>(result.Items),
            };

            return Ok(paged);
        }

        [HttpPost("OnChangePickingType")]
        public async Task<IActionResult> OnChangePickingType(StockPickingOnChangePickingType val)
        {
            var res = await _stockPickingService.OnChangePickingType(val);
            return Ok(res);
        }
    }
}