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
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleOrdersController : BaseApiController
    {
        private readonly ISaleOrderService _saleOrderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDotKhamService _dotKhamService;
        private readonly ICardCardService _cardService;
        private readonly IProductPricelistService _pricelistService;
        private readonly ISaleOrderLineService _saleLineService;

        public SaleOrdersController(ISaleOrderService saleOrderService, IMapper mapper,
            IUnitOfWorkAsync unitOfWork, IDotKhamService dotKhamService,
            ICardCardService cardService, IProductPricelistService pricelistService,
            ISaleOrderLineService saleLineService)
        {
            _saleOrderService = saleOrderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _dotKhamService = dotKhamService;
            _cardService = cardService;
            _pricelistService = pricelistService;
            _saleLineService = saleLineService;
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
            res.InvoiceCount = saleOrder.OrderLines.SelectMany(x => x.SaleOrderLineInvoice2Rels).Select(x => x.InvoiceLine.Move)
                .Where(x => x.Type == "out_invoice" || x.Type == "out_refund").Distinct().Count();
            res.OrderLines = res.OrderLines.OrderBy(x => x.Sequence);
            foreach (var inl in res.OrderLines)
            {
                inl.Teeth = inl.Teeth.OrderBy(x => x.Name);
            }

            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaleOrderSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var order = _mapper.Map<SaleOrder>(val);
            await SaveOrderLines(val, order);
            await _saleOrderService.CreateOrderAsync(order);

            var basic = _mapper.Map<SaleOrderBasic>(order);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, SaleOrderSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
       
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.UpdateOrderAsync(id, val);
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
        public async Task<IActionResult> DefaultGet([FromQuery]SaleOrderDefaultGet val)
        {
            var res = await _saleOrderService.DefaultGet(val);
            return Ok(res);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> CheckPromotion(Guid id)
        {
            //Kiem tra co chuong trinh khuyen mai nao co the ap dung cho don hang nay khong?
            var res = await _saleOrderService.CheckHasPromotionCanApply(id);
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

        [HttpPost("{id}/[action]")]
        public async Task<IActionResult> ActionConvertToOrder(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ActionConvertToOrder(id);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("{id}/[action]")]
        public async Task<IActionResult> ActionInvoiceCreateV2(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ActionInvoiceCreateV2(id);
            _unitOfWork.Commit();
            return NoContent();
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> ApplyCoupon(SaleOrderApplyCoupon val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ApplyCoupon(val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("{id}/[action]")]
        public async Task<IActionResult> ApplyPromotion(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.RecomputeCouponLines(new List<Guid>() { id });
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetPayments(Guid id)
        {
            var res = await _saleOrderService._GetPaymentInfoJson(id);
            return Ok(res);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetInvoices(Guid id)
        {
            var res = await _saleOrderService.GetInvoicesBasic(id);
            return Ok(res);
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

        [HttpPost("[action]")]
        public async Task<IActionResult> OnChangePartner(SaleOrderOnChangePartner val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var res = new SaleOrderOnChangePartnerResult();
            if (val.PartnerId.HasValue)
            {
                //tìm bảng giá mặc định
                var pricelist = await _pricelistService.SearchQuery(x => !x.CompanyId.HasValue, orderBy: x => x.OrderBy(s => s.Sequence)).FirstOrDefaultAsync();
                if (pricelist == null)
                {
                    var companyId = CompanyId;
                    pricelist = await _pricelistService.SearchQuery(x => x.CompanyId == companyId, orderBy: x => x.OrderBy(s => s.Sequence)).FirstOrDefaultAsync();
                }

                if (pricelist != null)
                    res.Pricelist = _mapper.Map<ProductPricelistBasic>(pricelist);

                var card = await _cardService.GetValidCard(val.PartnerId.Value);
                if (card != null && card.Type.PricelistId.HasValue)
                    res.Pricelist = await _pricelistService.GetBasic(card.Type.PricelistId.Value);
            }
          
            return Ok(res);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetPrint(Guid id)
        {
            var res = await _saleOrderService.GetPrint(id);
            res.OrderLines = res.OrderLines.OrderBy(x => x.Sequence);
            return Ok(res);
        }

        private async Task SaveOrderLines(SaleOrderSave val, SaleOrder order)
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

            if (lineToRemoves.Any())
                await _saleLineService.Unlink(lineToRemoves.Select(x => x.Id).ToList());

            //Cập nhật sequence cho tất cả các line của val
            int sequence = 0;
            foreach (var line in val.OrderLines)
            {
                if (line.Id == Guid.Empty)
                {
                    var saleLine = _mapper.Map<SaleOrderLine>(line);
                    saleLine.Sequence = sequence++;
                    foreach (var toothId in line.ToothIds)
                    {
                        saleLine.SaleOrderLineToothRels.Add(new SaleOrderLineToothRel
                        {
                            ToothId = toothId
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
                        saleLine.Sequence = sequence++;
                        saleLine.SaleOrderLineToothRels.Clear();
                        foreach (var toothId in line.ToothIds)
                        {
                            saleLine.SaleOrderLineToothRels.Add(new SaleOrderLineToothRel
                            {
                                ToothId = toothId
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

        [HttpPost("[action]")]
        public async Task<IActionResult> ApplyServiceCards(SaleOrderApplyServiceCards val)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ApplyServiceCards(val);
            _unitOfWork.Commit();
            return NoContent();
        }
    }
}