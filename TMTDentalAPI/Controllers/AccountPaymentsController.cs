using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountPaymentsController : BaseApiController
    {
        private readonly IAccountPaymentService _paymentService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public AccountPaymentsController(IAccountPaymentService paymentService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _paymentService = paymentService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]AccountPaymentPaged val)
        {
            var result = await _paymentService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var payment = await _paymentService.SearchQuery(x => x.Id == id).Include(x => x.Partner).Include(x => x.Journal)
                .FirstOrDefaultAsync();
            if (payment == null)
            {
                return NotFound();
            }

            var res = _mapper.Map<AccountPaymentDisplay>(payment);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _paymentService.CancelAsync(ids);
            _unitOfWork.Commit();
            return NoContent();
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> Unlink(IEnumerable<Guid> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _paymentService.UnlinkAsync(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpGet("GetPaymentBasicList")]
        public async Task<IActionResult> GetPaymentBasicList([FromQuery]AccountPaymentFilter val)
        {
            var res = await _paymentService.GetPaymentBasicList(val);
            return Ok(res);
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> Get(Guid id)
        //{
        //    var category = await _partnerCategoryService.SearchQuery(x => x.Id == id).Include(x => x.Parent).FirstOrDefaultAsync();
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(_mapper.Map<PartnerCategoryDisplay>(category));
        //}

        //[HttpPost]
        //public async Task<IActionResult> Create(PartnerCategoryDisplay val)
        //{
        //    if (null == val || !ModelState.IsValid)
        //        return BadRequest();

        //    var category = _mapper.Map<PartnerCategory>(val);
        //    await _partnerCategoryService.CreateAsync(category);

        //    return CreatedAtAction(nameof(Get), new { id = category.Id }, val);
        //}

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(Guid id, PartnerCategoryDisplay val)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest();
        //    var category = await _partnerCategoryService.GetByIdAsync(id);
        //    if (category == null)
        //        return NotFound();

        //    category = _mapper.Map(val, category);
        //    await _partnerCategoryService.UpdateAsync2(category);

        //    return NoContent();
        //}
    }
}