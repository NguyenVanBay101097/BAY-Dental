using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class ProductRequestsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IProductRequestService _productRequestervice;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public ProductRequestsController(IMapper mapper, IProductRequestService productRequestService, IUnitOfWorkAsync unitOfWork)
        {
            _mapper = mapper;
            _productRequestervice = productRequestService;
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ProductRequestPaged val)
        {
            var result = await _productRequestervice.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _productRequestervice.GetDisplay(id);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> DefaultGet(ProductRequestDefaultGet val)
        {
            var res = await _productRequestervice.DefaultGet(val);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductRequestSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            var request = await _productRequestervice.CreateRequest(val);

            _unitOfWork.Commit();

            var basic = _mapper.Map<ProductRequestBasic>(request);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ProductRequestSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _productRequestervice.UpdateRequest(id, val);

            _unitOfWork.Commit();

            return NoContent();
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var request = await _productRequestervice.GetByIdAsync(id);
            if (request.State != "draft")
                throw new Exception("Không thể xóa phiếu yêu cầu vật tư đang yêu cầu hoặc đã xuất");

            if (request == null)
                return NotFound();

            await _productRequestervice.DeleteAsync(request);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetLine(GetLinePar val)
        {
            var res = await _productRequestervice.Getline(val);
            return Ok(res);
        }

    }
}
