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
using System.IO;
using System.Linq;
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
            var jobService = GetService<ITCareJobService>();
            List<RecurringJobDto> list;
            var campaign = new TCareCampaign()
            {
                Name = val.Name,
                TCareScenarioId = val.TCareScenarioId,
                State = "draft"
            };
            //Cach 1:
            //var value = await _irConfigParameterService.GetParam("setup_first_time_create_campaign");
            //if (string.IsNullOrEmpty(value) || !value.Contains("True"))
            //{
            //    var tenant = _tenant != null ? _tenant.Hostname : "localhost";
            //    var jobId = $"{tenant}-tcare-scenario-{DateTime.Now.ToShortDateString()}";
            //    RecurringJob.RemoveIfExists(jobId);
            //    RecurringJob.AddOrUpdate(jobId, () => jobService.TCareTakeMessage(tenant), "10 16 * * *", TimeZoneInfo.Local);
            //    await _irConfigParameterService.SetParam("setup_first_time_create_campaign", "True");
            //}

            //Cach 2:
            var tenant = _tenant != null ? _tenant.Hostname : "localhost";
            var jobId = $"{tenant}-tcare-scenario";
            using (var connection = JobStorage.Current.GetConnection())
            {
                list = connection.GetRecurringJobs();
            }
            var job = list?.FirstOrDefault(j => j.Id == jobId);  // jobId is the recurring job ID, whatever that is
            if (job == null || string.IsNullOrEmpty(job.LastJobId))
            {
                RecurringJob.AddOrUpdate(jobId, () => jobService.TCareTakeMessage(tenant), "49 16 * * *", TimeZoneInfo.Local);
            }

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


            var runAt = campaign.SheduleStart.HasValue ? campaign.SheduleStart.Value : DateTime.Today;
            campaign.State = "running";
            campaign.Active = true;
            campaign.SheduleStart = runAt;
            var tenant = _tenant != null ? _tenant.Hostname : "localhost";
            var jobId = $"{tenant}-tcare-campaign-{campaign.Id}";
            campaign.RecurringJobId = jobId;
            RecurringJob.AddOrUpdate(campaign.RecurringJobId, () => jobService.Run(tenant, campaign.Id), $"{runAt.Minute} {runAt.Hour} * * *", TimeZoneInfo.Local);
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
                if (!string.IsNullOrEmpty(campaign.RecurringJobId))
                    RecurringJob.RemoveIfExists(campaign.RecurringJobId);
                campaign.RecurringJobId = null;
            }

            await UpdateAsync(campaigns);
        }

        public async Task SetSheduleStart(TCareCampaignSetSheduleStart val)
        {
            var campaign = await GetByIdAsync(val.Id);
            if (campaign != null)
            {
                campaign.SheduleStart = val.SheduleStart.HasValue ? val.SheduleStart.Value : DateTime.Today;
                await UpdateAsync(campaign);
            }
        }
    }
}
