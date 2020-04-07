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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Infrastructure.Services
{
    public class FacebookMassMessagingService : BaseService<FacebookMassMessaging>, IFacebookMassMessagingService
    {
        private readonly IMapper _mapper;
        private readonly IFacebookMessageSender _fbMessageSender;
        private readonly IFacebookMassMessageJobService _facebookMassMessageJobService;
        private readonly AppTenant _tenant;

        public FacebookMassMessagingService(IAsyncRepository<FacebookMassMessaging> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, IFacebookMessageSender fbMessageSender, IFacebookMassMessageJobService facebookMassMessageJobService,
            ITenant<AppTenant> tenant)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _fbMessageSender = fbMessageSender;
            _facebookMassMessageJobService = facebookMassMessageJobService;
            _tenant = tenant?.Value;
        }

        public async Task<PagedResult2<FacebookMassMessagingBasic>> GetPagedResultAsync(FacebookMassMessagingPaged val)
        {
            ISpecification<FacebookMassMessaging> spec = new InitialSpecification<FacebookMassMessaging>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<FacebookMassMessaging>(x => x.Name.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));

            var items = await _mapper.ProjectTo<FacebookMassMessagingBasic>(query).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<FacebookMassMessagingBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task ActionSend(IEnumerable<Guid> ids)
        {
            var userProfileObj = GetService<IFacebookUserProfileService>();
            var self = await SearchQuery(x => ids.Contains(x.Id) && x.State != "done").Include(x => x.FacebookPage).ToListAsync();
            foreach(var item in self)
            {
                if (item.FacebookPage == null)
                    continue;
                var page = item.FacebookPage;
                var profiles = await userProfileObj.SearchQuery(x => x.FbPageId == page.Id).ToListAsync();
                var tasks = profiles.Select(x => SendMessageAndTrace(item, item.Content, x, page.PageAccesstoken));

                await Task.WhenAll(tasks);

                item.State = "done";
                item.SentDate = DateTime.Now;
            }

            await UpdateAsync(self);
        }

        public async Task SendMessageAndTrace(FacebookMassMessaging self, string text, FacebookUserProfile profile, string access_token)
        {
            var traceObj = GetService<IFacebookMessagingTraceService>();
            var sendResult = await _fbMessageSender.SendMessageTagTextAsync(text, profile.PSID, access_token);
            if (sendResult == null)
            {
                await traceObj.CreateAsync(new FacebookMessagingTrace
                {
                    MassMessagingId = self.Id,
                    Exception = DateTime.Now,
                    UserProfileId = profile.Id
                });
            }
            else
            {
                await traceObj.CreateAsync(new FacebookMessagingTrace
                {
                    MassMessagingId = self.Id,
                    Sent = DateTime.Now,
                    MessageId = sendResult.message_id,
                    UserProfileId = profile.Id
                });
            }
        }

        public async Task<PagedResult2<FacebookUserProfileBasic>> ActionViewDelivered(Guid id, FacebookMassMessagingStatisticsPaged paged)
        {
            var traceObj = GetService<IFacebookMessagingTraceService>();
            var query = traceObj.SearchQuery(x => x.MassMessagingId == id && x.Delivered.HasValue);

            var items = await query.OrderByDescending(x => x.Delivered.Value).Skip(paged.Offset).Take(paged.Limit).Select(x => new FacebookUserProfileBasic
            {
                Name = x.UserProfile.Name,
                PSId = x.UserProfile.PSID
            }).ToListAsync();

            var total = await query.CountAsync();
            return new PagedResult2<FacebookUserProfileBasic>(total, paged.Limit, paged.Offset) { Items = items };
        }

        public async Task SetScheduleDate(FacebookMassMessagingSetScheduleDate val)
        {
            var self = await SearchQuery(x => x.Id == val.MassMessagingId).FirstOrDefaultAsync();
            self.ScheduleDate = val.ScheduleDate;
            self.State = "in_queue";

            var db = _tenant != null ? _tenant.Hostname : "localhost";
            var date = val.ScheduleDate.HasValue ? val.ScheduleDate.Value.ToUniversalTime() : DateTime.UtcNow;
            var jobId = BackgroundJob.Schedule(() => _facebookMassMessageJobService.SendMessage(db, self.Id), date);
            self.JobId = jobId;

            await UpdateAsync(self);
        }

        public async Task ActionCancel(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id) && x.State == "in_queue").ToListAsync();
            foreach(var item in self)
            {
                if (!string.IsNullOrEmpty(item.JobId))
                    BackgroundJob.Delete(item.JobId);
                item.State = "draft";
                item.ScheduleDate = null;
            }

            await UpdateAsync(self);
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            foreach (var item in self)
            {
                if (!string.IsNullOrEmpty(item.JobId))
                    BackgroundJob.Delete(item.JobId);
            }

            await DeleteAsync(self);
        }
    }

    [DataContract]
    public class AudienceFilter
    {
        /// <summary>
        /// Gets or sets the name of the sorted field (property). Set to <c>null</c> if the <c>Filters</c> property is set.
        /// </summary>
        [DataMember(Name = "field")]
        public string Field { get; set; }

        /// <summary>
        /// Gets or sets the filtering operator. Set to <c>null</c> if the <c>Filters</c> property is set.
        /// </summary>
        [DataMember(Name = "operator")]
        public string Operator { get; set; }

        /// <summary>
        /// Gets or sets the filtering value. Set to <c>null</c> if the <c>Filters</c> property is set.
        /// </summary>
        [DataMember(Name = "value")]
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the filtering logic. Can be set to "or" or "and". Set to <c>null</c> unless <c>Filters</c> is set.
        /// </summary>
        [DataMember(Name = "logic")]
        public string Logic { get; set; }

        /// <summary>
        /// Gets or sets the child filter expressions. Set to <c>null</c> if there are no child expressions.
        /// </summary>
        [DataMember(Name = "filters")]
        public IEnumerable<Filter> Filters { get; set; }
    }

    public class AudienceFilterItem
    {
        public string type { get; set; }
        public string name { get; set; }
        public string formula_type { get; set; }
        public string formula_value { get; set; }
        public string formula_display { get; set; }
    }
}
