using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class TCareCampaignService : BaseService<TCareCampaign>, ITCareCampaignService
    {
        private readonly IMapper _mapper;
        private readonly AppTenant _tenant;
        public TCareCampaignService(IAsyncRepository<TCareCampaign> repository, IHttpContextAccessor httpContextAccessor, ITenant<AppTenant> tenant,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _tenant = tenant?.Value;
            _mapper = mapper;
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
                State = "draft"
            };
            return await CreateAsync(campaign);
        }

        public async Task ActionStartCampaign(TCareCampaignStart val)
        {
            var jobService = GetService<ITCareJobService>();
            var states = new string[] { "draft", "stopped" };
            var campaign = await SearchQuery(x => x.Id == val.Id && states.Contains(x.State)).FirstOrDefaultAsync();

            campaign.State = "running";
            var runAt = val.SheduleStart.Value;
            var tenant = _tenant != null ? _tenant.Hostname : "localhost";
            if (campaign.RecurringJobId == null)
            {
                //đặt tên lưu lại trong hangfire
                campaign.RecurringJobId = $"{tenant}-{campaign.Name}-RecurringJob";

            }
            RecurringJob.AddOrUpdate(campaign.RecurringJobId, () => jobService.Run(_tenant != null ? _tenant.Hostname : "localhost", campaign.Id), $"{runAt.Minute} {runAt.Hour} * * *", TimeZoneInfo.Local);




            await UpdateAsync(campaign);
        }

        public async Task ActionStopCampaign(IEnumerable<Guid> ids)
        {
            var states = new string[] { "running" };
            var campaigns = await SearchQuery(x => ids.Contains(x.Id) && states.Contains(x.State)).ToListAsync();
            List<RecurringJobDto> list;
            foreach (var campaign in campaigns)
            {
                campaign.State = "stopped";
                campaign.SheduleStart = null;
                using (var connection = JobStorage.Current.GetConnection())
                {
                    //truy vấn danh sách RecurringJob
                    list = connection.GetRecurringJobs();
                }
                var job = list?.FirstOrDefault(j => j.Id == campaign.RecurringJobId);  // jobId is the recurring job ID, whatever that is
                if (job != null && !string.IsNullOrEmpty(job.LastJobId))
                {
                    BackgroundJob.Delete(job.LastJobId);
                    RecurringJob.RemoveIfExists(job.LastJobId);
                }
                RecurringJob.RemoveIfExists(job.Id = campaign.RecurringJobId);

                campaign.RecurringJobId = null;

            }


            await UpdateAsync(campaigns);
        }
    }
}
