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
    public class TCareMessagingsController : BaseApiController
    {
        private readonly ITCareMessagingService _tCareMessagingService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public TCareMessagingsController(ITCareMessagingService tCareMessagingService,
           IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _tCareMessagingService = tCareMessagingService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] TCareMessagingPaged val)
        {
            var result = await _tCareMessagingService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var mes = await _tCareMessagingService.GetDisplay(id);
            if (mes == null)
            {
                return NotFound();
            }
            return Ok(mes);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TCareMessagingSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var mes = await _tCareMessagingService.Create(val);
            var res = _mapper.Map<TCareMessagingBasic>(mes);
            _unitOfWork.Commit();
                              
            return Ok(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id ,TCareMessagingSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _tCareMessagingService.Update(id, val);          
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var type = await _tCareMessagingService.GetByIdAsync(id);
            if (type == null)
                return NotFound();
            await _tCareMessagingService.DeleteAsync(type);

            return NoContent();
        }

    }
}