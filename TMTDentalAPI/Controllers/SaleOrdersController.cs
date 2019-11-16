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
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class SaleOrdersController : ControllerBase
    {
        private readonly ISaleOrderService _saleOrderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDotKhamService _dotKhamService;

        public SaleOrdersController(ISaleOrderService saleOrderService, IMapper mapper,
            IUnitOfWorkAsync unitOfWork, IDotKhamService dotKhamService)
        {
            _saleOrderService = saleOrderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _dotKhamService = dotKhamService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]SaleOrderPaged val)
        {
            var result = await _saleOrderService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var saleOrder = await _saleOrderService.GetSaleOrderForDisplayAsync(id);
            if (saleOrder == null)
            {
                return NotFound();
            }

            var res = _mapper.Map<SaleOrderDisplay>(saleOrder);
            res.OrderLines = res.OrderLines.OrderBy(x => x.Sequence);
            foreach (var inl in res.OrderLines)
            {
                inl.Teeth = inl.Teeth.OrderBy(x => x.Name);
            }

            return Ok(_mapper.Map<SaleOrderDisplay>(saleOrder));
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaleOrderDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var order = _mapper.Map<SaleOrder>(val);
            order.OrderLines = _mapper.Map<ICollection<SaleOrderLine>>(val.OrderLines);
            await _saleOrderService.CreateOrderAsync(order);

            val.Id = order.Id;
            return CreatedAtAction(nameof(Get), new { id = order.Id }, val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, SaleOrderDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var order = await _saleOrderService.GetSaleOrderWithLines(id);
            if (order == null)
                return NotFound();

            order = _mapper.Map(val, order);

            SaveOrderLines(val, order);

            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.UpdateOrderAsync(order);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var saleOrder = await _saleOrderService.GetSaleOrderByIdAsync(id);
            if (saleOrder == null)
                return NotFound();
            await _saleOrderService.UnlinkSaleOrderAsync(saleOrder);

            return NoContent();
        }

        [HttpGet("DefaultGet")]
        public async Task<IActionResult> DefaultGet()
        {
            var res = await _saleOrderService.DefaultGet();
            return Ok(res);
        }

        [HttpPost("DefaultGetInvoice")]
        public IActionResult DefaultGetInvoice(List<Guid> ids)
        {
            var res =  _saleOrderService.DefaultGetInvoice(ids);
            return Ok(res);
        }

        [HttpPost("DefaultLineGet")]
        public async Task<IActionResult> DefaultLineGet(SaleOrderLineDefaultGet val)
        {
            var res = await _saleOrderService.DefaultLineGet(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            if (ids == null || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ActionCancel(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Unlink(IEnumerable<Guid> ids)
        {
            if (ids == null || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.Unlink(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetPrint(Guid id)
        {
            var res = await _saleOrderService.GetPrint(id);
            res.OrderLines = res.OrderLines.OrderBy(x => x.Sequence);
            return Ok(res);
        }

        private void SaveOrderLines(SaleOrderDisplay val, SaleOrder order)
        {
            var existLines = order.OrderLines.ToList();
            var lineToRemoves = new List<SaleOrderLine>();
            foreach (var existLine in existLines)
            {
                bool found = false;
                foreach (var item in val.OrderLines)
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
                if (line.State != "draft")
                    continue;
                order.OrderLines.Remove(line);
            }

            //Cập nhật sequence cho tất cả các line của val
            int sequence = 0;
            foreach (var line in val.OrderLines)
            {
                line.Sequence = sequence++;
            }

            foreach (var line in val.OrderLines)
            {
                if (line.Id == Guid.Empty)
                {
                    var saleLine = _mapper.Map<SaleOrderLine>(line);
                    foreach (var tooth in line.Teeth)
                    {
                        saleLine.SaleOrderLineToothRels.Add(new SaleOrderLineToothRel
                        {
                            ToothId = tooth.Id
                        });
                    }
                    order.OrderLines.Add(saleLine);
                }
                else
                {
                    var saleLine = order.OrderLines.SingleOrDefault(c => c.Id == line.Id);
                    if (saleLine != null)
                    {
                        _mapper.Map(line, saleLine);
                        saleLine.SaleOrderLineToothRels.Clear();
                        foreach (var tooth in line.Teeth)
                        {
                            saleLine.SaleOrderLineToothRels.Add(new SaleOrderLineToothRel
                            {
                                ToothId = tooth.Id
                            });
                        }
                    }
                }
            }
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> ActionConfirm(IEnumerable<Guid> ids)
        {
            if (ids == null || ids.Count() == 0)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ActionConfirm(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionDone(IEnumerable<Guid> ids)
        {
            if (ids == null || ids.Count() == 0)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ActionDone(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpGet("{id}/GetDotKhamList")]
        public async Task<IActionResult> GetDotKhamList(Guid id)
        {
            var dotKhams = await _dotKhamService.GetDotKhamsForSaleOrder(id);
            var res = _mapper.Map<IEnumerable<DotKhamBasic>>(dotKhams);
            return Ok(res);
        }
    }
}