using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TMTDentalAPI.JobFilters;
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


        ///Get{id}



        [HttpPost]
        public async Task<IActionResult> Create(MedicineOrderSave val)
        {

            await _unitOfWork.BeginTransactionAsync();

            var medicineOrder = await _medicineOrderService.CreateMedicineOrder(val);
            _unitOfWork.Commit();

            var display = _mapper.Map<MedicineOrderDisplay>(medicineOrder);
            return Ok(display);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, MedicineOrderSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _medicineOrderService.UpdateMedicineOrder(id, val);

            _unitOfWork.Commit();

            return NoContent();
        }

        ///DefaultGet

        [HttpPost("[action]")]
        public async Task<IActionResult> DefaultGet(DefaultGet val)
        {
            var result = await _medicineOrderService.DefaultGet(val);
            return Ok(result);
        }
    }
}
