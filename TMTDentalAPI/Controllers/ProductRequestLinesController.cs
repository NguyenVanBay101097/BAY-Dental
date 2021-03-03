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
    public class ProductRequestLinesController : BaseApiController
    {
        public IMapper _mapper;
        public IUnitOfWorkAsync _unitOfWork;
        public IProductRequestLineService _productRequestLineService;
        public ProductRequestLinesController(IMapper mapper, IUnitOfWorkAsync unitOfWork, IProductRequestLineService productRequestLineService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _productRequestLineService = productRequestLineService;
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> GetLine(GetLinePar val)
        {
            var res = await _productRequestLineService.GetlineAble(val);
            return Ok(res);
        }
    }
}
