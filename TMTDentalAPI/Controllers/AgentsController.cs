using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentsController : BaseApiController
    {
        private readonly IAgentService _agentService;
        private readonly IPartnerService _partnerService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public AgentsController(IAgentService agentService, IPartnerService partnerService, IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _agentService = agentService;
            _partnerService = partnerService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [CheckAccess(Actions = "Catalog.Agent.Read")]
        public async Task<IActionResult> Get([FromQuery] AgentPaged val)
        {
            var result = await _agentService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Catalog.Agent.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _agentService.GetDisplayById(id);
            return Ok(res);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Catalog.Agent.Read")]
        public async Task<IActionResult> GetCommissionAgent([FromQuery] CommissionAgentFilter val)
        {
            var result = await _agentService.GetCommissionAgent(val);
            return Ok(result);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Catalog.Agent.Read")]
        public async Task<IActionResult> GetCommissionAgentDetail([FromQuery] CommissionAgentDetailFilter val)
        {
            var result = await _agentService.GetCommissionAgentDetail(val);
            return Ok(result);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Catalog.Agent.Read")]
        public async Task<IActionResult> GetCommissionAgentDetailItem([FromQuery] CommissionAgentDetailItemFilter val)
        {
            var result = await _agentService.GetCommissionAgentDetailItem(val);
            return Ok(result);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetCommissionAmountAgent(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            var amountCommissionTotal = await _agentService.GetCommissionAmountAgent(id);
            _unitOfWork.Commit();

            return Ok(amountCommissionTotal);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetInComeAmountAgent(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            var amountInComeTotal = await _agentService.GetInComeAmountAgent(id);
            _unitOfWork.Commit();

            return Ok(amountInComeTotal);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetCommissionAgentBalance(Guid id, Guid partnerId )
        {
            await _unitOfWork.BeginTransactionAsync();
            var amountCommissionTotal = await _agentService.GetCommissionAgentBalance(id , partnerId);
            _unitOfWork.Commit();

            return Ok(amountCommissionTotal);
        }

        [HttpPost]
        [CheckAccess(Actions = "Catalog.Agent.Create")]
        public async Task<IActionResult> Create(AgentSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            var agent = _mapper.Map<Agent>(val);
            agent.CompanyId = CompanyId;

            await _agentService.CreateAsync(agent);

            _unitOfWork.Commit();

            var basic = _mapper.Map<AgentBasic>(agent);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Catalog.Agent.Update")]
        public async Task<IActionResult> Update(Guid id, AgentDisplay val)
        {

            var agent = await _agentService.SearchQuery(x => x.Id == id)
                .Include(x => x.Partner)
                .FirstOrDefaultAsync();

            if (agent == null)
                return NotFound();

            await _unitOfWork.BeginTransactionAsync();

            agent = _mapper.Map(val, agent);

            await UpdatePartnerToAgent(agent);

            await _agentService.UpdateAsync(agent);

            _unitOfWork.Commit();

            return NoContent();
        }

        private async Task UpdatePartnerToAgent(Agent agent)
        {
            if (agent.Partner == null)
                return;
            var pn = agent.Partner;
            pn.Name = agent.Name;
            pn.Phone = agent.Phone;
            await _partnerService.UpdateAsync(pn);
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Catalog.Agent.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();

            var agent = await _agentService.GetByIdAsync(id);
            if (agent == null)
                return NotFound();

            if (agent.Partner != null)
                await _partnerService.DeleteAsync(agent.Partner);

            await _agentService.DeleteAsync(agent);
            _unitOfWork.Commit();

            return NoContent();
        }
    }
}
