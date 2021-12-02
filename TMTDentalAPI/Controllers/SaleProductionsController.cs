using AutoMapper;
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
    public class SaleProductionsController : BaseApiController
    {
        private readonly ISaleProductionService _saleProductionService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public SaleProductionsController(ISaleProductionService saleProductionService, IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _saleProductionService = saleProductionService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateSaleProduction(UpdateSaleProductionReq val)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _saleProductionService.UpdateSaleProduction(val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _saleProductionService.Unlink(new List<Guid>() { id });
            _unitOfWork.Commit();
            return NoContent();
        }
    }
}
