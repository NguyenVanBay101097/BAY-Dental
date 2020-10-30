using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
using Hangfire;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class TCareScenarioService : BaseService<TCareScenario>, ITCareScenarioService
    {
        private readonly IMapper _mapper;
        private readonly AppTenant _tenant;
        public TCareScenarioService(IAsyncRepository<TCareScenario> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, ITenant<AppTenant> tenant
            ) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _tenant = tenant?.Value;

        }

        public override async Task UpdateAsync(TCareScenario entity)
        {
            var campaignObj = GetService<ITCareCampaignService>();
            await base.UpdateAsync(entity);
            var campaigns = await campaignObj.SearchQuery(x => x.TCareScenarioId == entity.Id).ToListAsync();
            foreach (var item in campaigns)
            {
                item.FacebookPageId = entity.ChannelSocialId;
            }
            await campaignObj.UpdateAsync(campaigns);
            await AddOrUpdateRunningJob(entity);
        }

        public override async Task<TCareScenario> CreateAsync(TCareScenario entity)
        {
            var model = await base.CreateAsync(entity);

            var campaignObj = GetService<ITCareCampaignService>();
            var campaigns = await campaignObj.SearchQuery(x => x.TCareScenarioId == entity.Id).ToListAsync();
            foreach (var item in campaigns)
            {
                item.FacebookPageId = entity.ChannelSocialId;
            }

            await campaignObj.UpdateAsync(campaigns);
            await AddOrUpdateRunningJob(entity);

            return model;
        }

        public async Task AddOrUpdateRunningJob(TCareScenario entity)
        {
            var tenant = _tenant != null ? _tenant.Hostname : "localhost";
            var jobId = $"{tenant}-tcare-scenario-{entity.Id}-custom";

            entity.JobId = jobId;
            if (entity.Type == "auto_custom")
            {
                if (entity.AutoCustomType == "custom1")
                    RecurringJob.AddOrUpdate<TCareScenarioJobService>(jobId, x => x.Run(tenant, new Guid[] { entity.Id }), $"{entity.CustomMinute} {entity.CustomHour} {entity.CustomDay} {entity.CustomMonth} *", TimeZoneInfo.Local);
                else if (entity.AutoCustomType == "custom2")
                    RecurringJob.AddOrUpdate<TCareScenarioJobService>(jobId, x => x.Run(tenant, new Guid[] { entity.Id }), $"{entity.CustomMinute} {entity.CustomHour} {entity.CustomDay} * *", TimeZoneInfo.Local);
            }
            else if (entity.Type == "manual" || entity.Type == "auto_everyday")
            {
                if (!string.IsNullOrEmpty(entity.JobId))
                    RecurringJob.RemoveIfExists(entity.JobId);
                entity.JobId = null;
            }

            await base.UpdateAsync(entity);
        }

        public async Task<PagedResult2<TCareScenarioBasic>> GetPagedResultAsync(TCareScenarioPaged val)
        {
            ISpecification<TCareScenario> spec = new InitialSpecification<TCareScenario>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<TCareScenario>(x => x.Name.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await _mapper.ProjectTo<TCareScenarioBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();
            var totalItems = await query.CountAsync();
            return new PagedResult2<TCareScenarioBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task ActionStart(IEnumerable<Guid> ids)
        {
            var scenarioJobObj = GetService<ITCareScenarioJobService>();
            var db = _tenant != null ? _tenant.Hostname : "localhost";
            await scenarioJobObj.Run(db, ids);
        }

        public async Task<TCareScenarioDisplay> GetDisplay(Guid id)
        {
            var result = await SearchQuery(x => x.Id == id).Include(x => x.Campaigns).Include(x => x.ChannelSocial).FirstOrDefaultAsync();
            var res = _mapper.Map<TCareScenarioDisplay>(result);
            return res;
        }

        public async Task<IEnumerable<TCareScenarioBasic>> GetAutocompleteAsync(TCareScenarioPaged val)
        {
            ISpecification<TCareScenario> spec = new InitialSpecification<TCareScenario>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<TCareScenario>(x => x.Name.Contains(val.Search)));
            //if (!string.IsNullOrEmpty(val.Type))
            //    spec = spec.And(new InitialSpecification<PartnerSource>(x => x.Type == val.Type));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));
            var items = await _mapper.ProjectTo<TCareScenarioBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();

            return items;
        }
    }
    public class ReportTCareCampaign
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int DeliveryTotal { get; set; }
        public int ReadTotal { get; set; }
    }
}
