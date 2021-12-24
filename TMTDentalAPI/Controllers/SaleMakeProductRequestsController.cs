using ApplicationCore.Entities;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleMakeProductRequestsController : BaseApiController
    {
        private readonly IProductRequestService _productRequestService;
        private readonly ISaleProductionLineService _saleProductionLineService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public SaleMakeProductRequestsController(IProductRequestService productRequestService,
            ISaleProductionLineService saleProductionLineService,
            IUnitOfWorkAsync unitOfWork)
        {
            _productRequestService = productRequestService;
            _saleProductionLineService = saleProductionLineService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaleMakeProductRequestVM val)
        {
            if (!val.Lines.Any())
                throw new Exception("Bạn chưa chọn vật tư để yêu cầu");

            var productRequest = new ProductRequest()
            {
                UserId = val.UserId,
                EmployeeId = val.EmployeeId,
                CompanyId = CompanyId,
                SaleOrderId = val.SaleOrderId,
                Date = val.Date
            };

            foreach (var line in val.Lines)
            {
                var saleProductionLine = await _saleProductionLineService.GetByIdAsync(line.SaleProductionLineId);
                var requestLine = new ProductRequestLine
                {
                    ProductId = saleProductionLine.ProductId,
                    ProductUOMId = saleProductionLine.ProductUOMId,
                    ProductQty = line.ProductQty,
                };
                requestLine.SaleProductionLineRels.Add(new SaleProductionLineProductRequestLineRel { SaleProductionLineId = line.SaleProductionLineId });

                productRequest.Lines.Add(requestLine);
            }

            await _unitOfWork.BeginTransactionAsync();

            await _productRequestService.CreateAsync(productRequest);
            await _productRequestService.ActionConfirm(new List<Guid>() { productRequest.Id });

            _unitOfWork.Commit();

            return NoContent();
        }
    }
}
