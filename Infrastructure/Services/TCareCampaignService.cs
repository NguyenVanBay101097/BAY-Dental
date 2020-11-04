using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class TCareCampaignService : BaseService<TCareCampaign>, ITCareCampaignService
    {
        private readonly IMapper _mapper;
        private readonly AppTenant _tenant;
        private readonly IIrConfigParameterService _irConfigParameterService;
        public TCareCampaignService(IAsyncRepository<TCareCampaign> repository, IHttpContextAccessor httpContextAccessor, ITenant<AppTenant> tenant,
            IMapper mapper, IIrConfigParameterService irConfigParameterService)
            : base(repository, httpContextAccessor)
        {
            _tenant = tenant?.Value;
            _mapper = mapper;
            _irConfigParameterService = irConfigParameterService;
        }

        public override async Task<IEnumerable<TCareCampaign>> CreateAsync(IEnumerable<TCareCampaign> entities)
        {
            await _SetFacebookPage(entities);
            return await base.CreateAsync(entities);
        }

        public override async Task UpdateAsync(IEnumerable<TCareCampaign> entities)
        {
            await _SetFacebookPage(entities);
            await base.UpdateAsync(entities);
        }

        private async Task _SetFacebookPage(IEnumerable<TCareCampaign> self)
        {
            var scenarioObj = GetService<ITCareScenarioService>();
            foreach(var campaign in self)
            {
                if (!campaign.FacebookPageId.HasValue)
                {
                    if (campaign.TCareScenarioId.HasValue)
                    {
                        var scenario = await scenarioObj.GetByIdAsync(campaign.TCareScenarioId.Value);
                        campaign.FacebookPageId = scenario.ChannelSocialId;
                    }
                }    
            }
        }

        public async Task<PagedResult2<TCareCampaignBasic>> GetPagedResultAsync(TCareCampaignPaged val)
        {
            ISpecification<TCareCampaign> spec = new InitialSpecification<TCareCampaign>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<TCareCampaign>(x => x.Name.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await _mapper.ProjectTo<TCareCampaignBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<TCareCampaignBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<TCareCampaign> NameCreate(TCareCampaignNameCreateVM val)
        {
            var campaign = new TCareCampaign()
            {
                Name = val.Name,
                TCareScenarioId = val.TCareScenarioId,
                State = "draft"
            };
            return await CreateAsync(campaign);
        }

        public async Task ActionStartCampaign(TCareCampaignStart val)
        {
            var jobService = GetService<ITCareJobService>();
            var states = new string[] { "draft", "stopped" };
            var campaign = await SearchQuery(x => x.Id == val.Id && x.Active == false).FirstOrDefaultAsync();

            XmlSerializer serializer = new XmlSerializer(typeof(MxGraphModel));
            MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(campaign.GraphXml));
            MxGraphModel resultingMessage = (MxGraphModel)serializer.Deserialize(memStream);
            var sequence = resultingMessage.Root.Sequence;
            var rule = resultingMessage.Root.Rule;
            if (sequence == null || rule == null)
                throw new Exception("điều kiện hoặc nội dung trống");
            campaign.State = "running";
            campaign.Active = true;                    
            await UpdateAsync(campaign);
        }

        public async Task ActionStopCampaign(IEnumerable<Guid> ids)
        {
            var states = new string[] { "running" };
            var campaigns = await SearchQuery(x => ids.Contains(x.Id) && states.Contains(x.State)).ToListAsync();
            foreach (var campaign in campaigns)
            {
                campaign.State = "stopped";
                campaign.Active = false;
            }

            await UpdateAsync(campaigns);
        }

        public async Task SetSheduleStart(TCareCampaignSetSheduleStart val)
        {
            var campaign = await GetByIdAsync(val.Id);
            if (campaign != null)
            {
                campaign.SheduleStartType = val.ScheduleStartType;
                campaign.SheduleStartNumber = val.ScheduleStartNumber;
                await UpdateAsync(campaign);
            }
        }
    }
}
