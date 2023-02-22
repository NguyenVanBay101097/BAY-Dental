using ApplicationCore.Entities;
using ApplicationCore.Users;
using Ardalis.ApiEndpoints;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Endpoints.ProductEndpoints
{
    [Authorize]
    public class DeleteService : BaseAsyncEndpoint
    .WithRequest<GetServiceRequest>
    .WithoutResponse
    {
        private readonly IProductService _productService;
        private readonly IProductStepService _productStepService;
        private readonly IProductBomService _productBomService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public DeleteService(IProductService productService,
            IProductStepService productStepService,
            IProductBomService productBomService,
            IUnitOfWorkAsync unitOfWork)
        {
            _productService = productService;
            _productBomService = productBomService;
            _productStepService = productStepService;
            _unitOfWork = unitOfWork;
        }

        [HttpDelete("api/ServiceProducts/{Id}")]
        public override async Task<ActionResult> HandleAsync([FromRoute] GetServiceRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var steps = await _productStepService.SearchQuery(x => x.ProductId == request.Id).ToListAsync();
                await _productStepService.DeleteAsync(steps);

                var boms = await _productBomService.SearchQuery(x => x.ProductId == request.Id).ToListAsync();
                await _productBomService.DeleteAsync(boms);

                var product = await _productService.SearchQuery(x => x.Id == request.Id).FirstOrDefaultAsync();
                await _productService.DeleteAsync(product);

                _unitOfWork.Commit();

                return NoContent();
            }
            catch
            {
                throw new Exception("Không thể xóa dịch vụ do có ràng buộc dữ liệu");
            }
        }
    }
}
