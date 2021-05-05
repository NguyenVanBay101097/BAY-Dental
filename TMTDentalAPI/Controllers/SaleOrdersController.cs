using System;
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
    public class SaleOrdersController : BaseApiController
    {
        private readonly ISaleOrderService _saleOrderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDotKhamService _dotKhamService;
        private readonly ICardCardService _cardService;
        private readonly IProductPricelistService _pricelistService;
        private readonly ISaleOrderLineService _saleLineService;
        private readonly IViewRenderService _viewRenderService;

        public SaleOrdersController(ISaleOrderService saleOrderService, IMapper mapper,
            IUnitOfWorkAsync unitOfWork, IDotKhamService dotKhamService,
            ICardCardService cardService, IProductPricelistService pricelistService,
            ISaleOrderLineService saleLineService, IViewRenderService viewRenderService)
        {
            _saleOrderService = saleOrderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _dotKhamService = dotKhamService;
            _cardService = cardService;
            _pricelistService = pricelistService;
            _saleLineService = saleLineService;
            _viewRenderService = viewRenderService;
        }

        [HttpGet]
        [CheckAccess(Actions = "Basic.SaleOrder.Read")]
        public async Task<IActionResult> Get([FromQuery] SaleOrderPaged val)
        {
            var result = await _saleOrderService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Basic.SaleOrder.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _saleOrderService.GetSaleOrderForDisplayAsync(id);
            if (res == null)
                return NotFound();

            //var res = _mapper.Map<SaleOrderDisplay>(res);
            //res.InvoiceCount = res.OrderLines.SelectMany(x => x.SaleOrderLineInvoice2Rels).Select(x => x.InvoiceLine.Move)
            //    .Where(x => x.Type == "out_invoice" || x.Type == "out_refund").Distinct().Count();
            //res.OrderLines = res.OrderLines.OrderBy(x => x.Sequence);
            //foreach (var inl in res.OrderLines)
            //{
            //    inl.Teeth = inl.Teeth.OrderBy(x => x.Name);
            //}

            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Basic.SaleOrder.Create")]
        public async Task<IActionResult> Create(SaleOrderSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var order = await _saleOrderService.CreateOrderAsync(val);
            _unitOfWork.Commit();

            var basic = _mapper.Map<SaleOrderBasic>(order);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Basic.SaleOrder.Update")]
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
        [CheckAccess(Actions = "Basic.SaleOrder.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var saleOrder = await _saleOrderService.GetSaleOrderByIdAsync(id);
            if (saleOrder == null)
                return NotFound();
            await _saleOrderService.UnlinkSaleOrderAsync(saleOrder);

            return NoContent();
        }

        [HttpGet("DefaultGet")]
        public async Task<IActionResult> DefaultGet([FromQuery] SaleOrderDefaultGet val)
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
            var res = _saleOrderService.DefaultGetInvoice(ids);
            return Ok(res);
        }

        [HttpPost("DefaultLineGet")]
        public async Task<IActionResult> DefaultLineGet(SaleOrderLineDefaultGet val)
        {
            var res = await _saleOrderService.DefaultLineGet(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Cancel")]
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
        [CheckAccess(Actions = "Basic.SaleOrder.Update,Basic.SaleOrder.Create")]
        public async Task<IActionResult> ActionConvertToOrder(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            var order = await _saleOrderService.ActionConvertToOrder(id);
            _unitOfWork.Commit();

            var basic = _mapper.Map<SaleOrderBasic>(order);
            return Ok(basic);
        }

        [HttpPost("{id}/[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Update")]
        public async Task<IActionResult> ActionInvoiceCreateV2(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ActionInvoiceCreateV2(id);
            _unitOfWork.Commit();
            return NoContent();
        }


        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Update")]
        public async Task<IActionResult> ApplyCouponOnOrder(ApplyPromotionUsageCode val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var res = await _saleOrderService.ApplyCoupon(val);
            _unitOfWork.Commit();
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Update")]
        public async Task<IActionResult> ApplyDiscountOnOrder(ApplyDiscountViewModel val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ApplyDiscountOnOrder(val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Update")]
        public async Task<IActionResult> ApplyPromotion(ApplyPromotionRequest val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ApplyPromotionOnOrder(val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpGet("{id}/[action]")]
        [CheckAccess(Actions = "Basic.AccountPayment.Read")]
        public async Task<IActionResult> GetPayments(Guid id)
        {
            var res = await _saleOrderService._GetPaymentInfoJson(id);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Delete")]
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
        [CheckAccess(Actions = "Basic.SaleOrder.Read")]
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
        [CheckAccess(Actions = "Basic.SaleOrder.Update")]
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
        [CheckAccess(Actions = "Basic.SaleOrder.Update")]
        public async Task<IActionResult> ActionDone(IEnumerable<Guid> ids)
        {
            if (ids == null || ids.Count() == 0)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ActionDone(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Update")]
        public async Task<IActionResult> ActionUnlock(IEnumerable<Guid> ids)
        {
            if (ids == null || ids.Count() == 0)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ActionUnlock(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpGet("{id}/GetDotKhamList")]
        [CheckAccess(Actions = "Basic.DotKham.Read")]
        public async Task<IActionResult> GetDotKhamList(Guid id)
        {
            var res = await _dotKhamService.GetDotKhamBasicsForSaleOrder(id);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Update")]
        public async Task<IActionResult> ApplyServiceCards(SaleOrderApplyServiceCards val)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ApplyServiceCards(val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ApplyDiscountDefault(ApplyDiscountViewModel val)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ApplyDiscountDefault(val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Create")]
        public async Task<IActionResult> CreateFastSaleOrder(FastSaleOrderSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var res = await _saleOrderService.CreateFastSaleOrder(val);
            _unitOfWork.Commit();
            return Ok(res);
        }


        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetServiceBySaleOrderId(Guid id)
        {
            var res = await _saleOrderService.GetServiceBySaleOrderId(id);
            return Ok(res);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetTreatmentBySaleOrderId(Guid id)
        {
            var res = await _saleOrderService.GetTreatmentBySaleOrderId(id);
            return Ok(res);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetLaboBySaleOrderId(Guid id)
        {
            var res = await _saleOrderService.GetLaboBySaleOrderId(id);
            return Ok(res);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetSaleOrderPaymentBySaleOrderId(Guid id)
        {
            var res = await _saleOrderService.GetSaleOrderPaymentBySaleOrderId(id);
            return Ok(res);
        }

        [AllowAnonymous]
        [HttpGet("{id}/[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Read")]
        public async Task<IActionResult> GetPrintSaleOrder(Guid id)
        {
            var phieu = await _saleOrderService.GetPrint(id);
            if (phieu == null)
                return NotFound();

            var html = _viewRenderService.Render("SaleOrder/Print", phieu);

            return Ok(new PrintData() { html = html });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ToSurvey([FromBody] SaleOrderToSurveyFilter val)
        {
            var paged = await _saleOrderService.GetToSurveyPagedAsync(val);
            return Ok(paged);
        }

        [HttpPost("{id}/[action]")]
        public async Task<IActionResult> GetLineForProductRequest(Guid id)
        {
            var res = await _saleOrderService.GetLineForProductRequest(id);
            return Ok(res);
        }
    }
}