using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Dapper;
using Infrastructure.Data;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SaasKit.Multitenancy;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.OdataControllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class SaleOrdersController : BaseController
    {
        private readonly ISaleOrderService _saleOrderService;
        private readonly IDotKhamService _dotKhamService;
        private readonly IMapper _mapper;
        private readonly IProductPricelistService _pricelistService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly ICardCardService _cardService;

        public SaleOrdersController(ISaleOrderService saleOrderService, IDotKhamService dotKhamService,
            IProductPricelistService pricelistService, ICardCardService cardService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _saleOrderService = saleOrderService;
            _dotKhamService = dotKhamService;
            _mapper = mapper;
            _pricelistService = pricelistService;
            _unitOfWork = unitOfWork;
            _cardService = cardService;
        }

        [EnableQuery]
        [HttpGet]
        public IActionResult Get()
        {
            var results = _mapper.ProjectTo<SaleOrderViewModel>(_saleOrderService.SearchQuery());
            return Ok(results);
        }

        [EnableQuery]
        [HttpGet]
        public SingleResult<SaleOrderViewModel> Get([FromODataUri] Guid key)
        {
            var results = _mapper.ProjectTo<SaleOrderViewModel>(_saleOrderService.SearchQuery(x => x.Id == key));
            return SingleResult.Create(results);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetDisplay([FromODataUri] Guid key)
        {
            var res = await _saleOrderService.GetDisplayAsync(key);
            if (res == null)
            {
                return NotFound();
            }
            return Ok(res);
        }


        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> GetSaleOrderLines([FromODataUri] Guid key)
        {
            var res = await _saleOrderService.GetSaleOrderLineBySaleOrder(key);
            if (res == null)
            {
                return NotFound();
            }
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> DefaultGet([FromQuery] SaleOrderDefaultGet val)
        {
            var res = await _saleOrderService.DefaultGet(val);
            return Ok(res);
        }

        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> GetDotKhamStepByOrderLine([FromODataUri] Guid key)
        {
            var res = await _saleOrderService.GetDotKhamStepByOrderLine(key);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Post(SaleOrderSave model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _unitOfWork.BeginTransactionAsync();
            var order = await _saleOrderService.CreateOrderAsync(model);
            _unitOfWork.Commit();

            var basic = _mapper.Map<SaleOrderViewModel>(order);
            return Created(basic);
        }

        [HttpPut]
        public async Task<IActionResult> PUT([FromODataUri] Guid key, SaleOrderSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.UpdateOrderAsync(key, val);
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


        [HttpGet("[action]")]
        public async Task<IActionResult> GetDotKhamListIds([FromODataUri] Guid key)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            //return ve list<Guid>;
            var dotKhamIds = await _dotKhamService.SearchQuery(x => x.SaleOrderId == key).OrderByDescending(x => x.Sequence).Select(x => x.Id).ToListAsync();

            return Ok(dotKhamIds);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDotKham([FromODataUri] Guid key, [FromBody] DotKhamSaveVm val)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _unitOfWork.BeginTransactionAsync();
            var res = await _dotKhamService.CreateDotKham(key, val);
            var viewdotkham = _mapper.Map<DotKhamVm>(res);
            _unitOfWork.Commit();

            return Ok(viewdotkham);
        }

        [HttpPost]
        public async Task<IActionResult> ActionConvertToOrder([FromODataUri]Guid key)
        {
            await _unitOfWork.BeginTransactionAsync();
            var order = await _saleOrderService.ActionConvertToOrder(key);
            _unitOfWork.Commit();

            var basic = _mapper.Map<SaleOrderBasic>(order);
            return Ok(basic);
        }

        [HttpPost]
        public async Task<IActionResult> ActionDone([FromBody] ActionDonePar val)
        {
            if (val.Ids == null || val.Ids.Count() == 0)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ActionDone(val.Ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> ActionConfirm([FromBody] ActionDonePar val)
        {
            if (val.Ids == null || val.Ids.Count() == 0)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ActionConfirm(val.Ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> OnChangePartner([FromBody]SaleOrderOnChangePartner val)
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
    }
}
