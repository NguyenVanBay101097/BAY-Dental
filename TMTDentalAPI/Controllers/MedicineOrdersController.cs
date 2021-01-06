using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Utilities;
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
        private readonly IViewRenderService _viewRenderService;

        public MedicineOrdersController(IMedicineOrderService medicineOrderService, IMapper mapper, IUnitOfWorkAsync unitOfWork,
            IViewRenderService viewRenderService)
        {
            _medicineOrderService = medicineOrderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _viewRenderService = viewRenderService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] MedicineOrderPaged val)
        {
            var result = await _medicineOrderService.GetPagedResultAsync(val);
            return Ok(result);
        }


        ///Get{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _medicineOrderService.GetByIdDisplay(id);
            return Ok(res);
        }



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

        [HttpGet("{id}/[action]")]
        [JobFilters.CheckAccess(Actions = "Basic.LaboOrder.Read")]
        public async Task<IActionResult> GetPrint(Guid id)
        {
            //get viewmodel và truyền vào view

            var res = await _medicineOrderService.GetPrint(id);

            var html = _viewRenderService.Render("MedicineOrder/Print", res);

            return Ok(new PrintData() { html = html });
        }
    }
}
