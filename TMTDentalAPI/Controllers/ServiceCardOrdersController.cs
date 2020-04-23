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
    public class ServiceCardOrdersController : BaseApiController
    {
        private readonly IServiceCardOrderService _cardOrderService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public ServiceCardOrdersController(IServiceCardOrderService cardOrderService,
            IMapper mapper, IUserService userService, IUnitOfWorkAsync unitOfWork)
        {
            _cardOrderService = cardOrderService;
            _mapper = mapper;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]ServiceCardOrderPaged val)
        {
            var res = await _cardOrderService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var order = await _cardOrderService.SearchQuery(x => x.Id == id).Include(x => x.Partner)
                .Include(x => x.User).Include(x => x.CardType).FirstOrDefaultAsync();

            if (order == null)
                return NotFound();

            return Ok(_mapper.Map<ServiceCardOrderDisplay>(order));
        }

        [HttpPost]
        public async Task<IActionResult> Create(ServiceCardOrderSave val)
        {
            var order = await _cardOrderService.CreateUI(val);

            var basic = _mapper.Map<ServiceCardOrderBasic>(order);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ServiceCardOrderSave val)
        {
            await _cardOrderService.UpdateUI(id, val);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var order = await _cardOrderService.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            await _cardOrderService.DeleteAsync(order);

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionConfirm(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _cardOrderService.ActionConfirm(ids);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> DefaultGet()
        {
            var user = await _userService.GetCurrentUser();
            var res = new ServiceCardOrderDefault();
            res.User = _mapper.Map<ApplicationUserSimple>(user);

            return Ok(res);
        }

        [HttpPost("{id}/[action]")]
        public async Task<IActionResult> AddPartners(Guid id, IEnumerable<Guid> ids)
        {
            await _cardOrderService.AddPartners(id, ids);
            return NoContent();
        }

        [HttpPost("{id}/[action]")]
        public async Task<IActionResult> RemovePartners(Guid id, IEnumerable<Guid> ids)
        {
            await _cardOrderService.RemovePartners(id, ids);
            return NoContent();
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetPartners(Guid id)
        {
            var order = await _cardOrderService.SearchQuery(x => x.Id == id).Include(x => x.PartnerRels)
               .Include("PartnerRels.Partner").FirstOrDefaultAsync();

            if (order == null)
                return NotFound();

            var partners = order.PartnerRels.Select(x => x.Partner);
            return Ok(_mapper.Map<IEnumerable<PartnerSimple>>(partners));
        }
    }
}