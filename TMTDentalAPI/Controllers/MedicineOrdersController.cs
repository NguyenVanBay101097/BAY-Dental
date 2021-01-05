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
    public class MedicineOrdersController : BaseApiController
    {
        private readonly IMedicineOrderService _medicineOrderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public MedicineOrdersController(IMedicineOrderService medicineOrderService, IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _medicineOrderService = medicineOrderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]     
        public async Task<IActionResult> Get([FromQuery] MedicineOrderPaged val)
        {
            var result = await _medicineOrderService.GetPagedResultAsync(val);
            return Ok(result);
        }
    }
}
