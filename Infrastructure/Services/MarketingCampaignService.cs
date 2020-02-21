using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class MarketingCampaignService : BaseService<MarketingCampaign>, IMarketingCampaignService
    {
        private readonly IMapper _mapper;
        private readonly AppTenant _tenant;
        private readonly IMarketingCampaignActivityJobService _activityJobService;

        public MarketingCampaignService(IAsyncRepository<MarketingCampaign> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, ITenant<AppTenant> tenant, IMarketingCampaignActivityJobService activityJobService)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _tenant = tenant?.Value;
            _activityJobService = activityJobService;
        }

        public async Task<PagedResult2<MarketingCampaignBasic>> GetPagedResultAsync(MarketingCampaignPaged val)
        {
            ISpecification<MarketingCampaign> spec = new InitialSpecification<MarketingCampaign>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<MarketingCampaign>(x => x.Name.Contains(val.Search)));
            if (!string.IsNullOrEmpty(val.State))
                spec = spec.And(new InitialSpecification<MarketingCampaign>(x => x.State == val.State));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));
            var items = await _mapper.ProjectTo<MarketingCampaignBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<MarketingCampaignBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task ActionStartCampaign(IEnumerable<Guid> ids)
        {
            var states = new string[] { "draft", "stopped" };
            var self = await SearchQuery(x => ids.Contains(x.Id) && states.Contains(x.State)).Include(x => x.Activities).ToListAsync();
            foreach(var campaign in self)
            {
                campaign.State = "running";
                campaign.DateStart = DateTime.Now;
                foreach(var activity in campaign.Activities)
                {
                    var intervalNumber = activity.IntervalNumber ?? 0;
                    var cronExpression = "";
                    if (activity.TriggerType == "everyday" && activity.EveryDayTimeAt.HasValue)
                    {
                        var hour = activity.EveryDayTimeAt.Value.Hour;
                        var minute = activity.EveryDayTimeAt.Value.Minute;
                        cronExpression = $"{minute} {hour} * * *";
                    }
                    else
                    {
                        if (activity.IntervalType == "hours")
                            cronExpression = $"* */{intervalNumber} * * *";
                        else if (activity.IntervalType == "days")
                            cronExpression = $"* * */{intervalNumber} * *";
                        else if (activity.IntervalType == "months")
                            cronExpression = $"* * * */{intervalNumber} *";
                        else if (activity.IntervalType == "weeks")
                            cronExpression = $"* * */{intervalNumber * 7} * *";
                    }

                    RecurringJob.AddOrUpdate($"{_tenant.Hostname}-activity-{activity.Id}", () => _activityJobService.RunActivity(_tenant.Hostname, activity.Id), cronExpression, TimeZoneInfo.Local);
                }
            }

            await UpdateAsync(self);
        }

        public async Task ActionStopCampaign(IEnumerable<Guid> ids)
        {
            var states = new string[] { "running" };
            var self = await SearchQuery(x => ids.Contains(x.Id) && states.Contains(x.State)).Include(x => x.Activities).ToListAsync();
            foreach (var campaign in self)
            {
                campaign.State = "stopped";
                foreach (var activity in campaign.Activities)
                {
                    RecurringJob.RemoveIfExists($"{_tenant.Hostname}-activity-{activity.Id}");
                }
            }

            await UpdateAsync(self);
        }
    }
}
