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
using System.Linq.Expressions;

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
            var userObj = GetService<IUserService>();
            var user = await userObj.GetCurrentUser();
            ISpecification<FacebookMassMessaging> spec = new InitialSpecification<FacebookMassMessaging>(x => x.FacebookPageId == user.FacebookPageId);

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
                var profiles = GetProfilesSendMessage(item, page);
                var tasks = profiles.Select(x => SendMessageAndTrace(item, item.Content, x, page.PageAccesstoken));

                await Task.WhenAll(tasks);

                item.State = "done";
                item.SentDate = DateTime.Now;
            }

            await UpdateAsync(self);
        }

        private IEnumerable<FacebookUserProfile> GetProfilesSendMessage(FacebookMassMessaging self, FacebookPage page)
        {
            //Lấy ra những profiles sẽ gửi message
            ISpecification<FacebookUserProfile> profileSpec = new InitialSpecification<FacebookUserProfile>(x => x.FbPageId == page.Id);
            if (!string.IsNullOrEmpty(self.AudienceFilter))
            {
                var filter = JsonConvert.DeserializeObject<SimpleFilter>(self.AudienceFilter);
                if (filter.items.Any())
                {
                    var itemSpecs = new List<ISpecification<FacebookUserProfile>>();
                    foreach(var item in filter.items)
                    {
                        var parameter = Expression.Parameter(typeof(FacebookUserProfile), "x");
                        var expression = ToLamdaExpression(item, parameter);

                        var predicateExpression = Expression.Lambda<Func<FacebookUserProfile, bool>>(expression, parameter);
                        itemSpecs.Add(new InitialSpecification<FacebookUserProfile>(predicateExpression));
                    }

                    if (filter.type == "and")
                    {
                        ISpecification<FacebookUserProfile> tmp = new InitialSpecification<FacebookUserProfile>(x => true);
                        foreach (var spec in itemSpecs)
                            tmp = tmp.And(spec);
                        profileSpec = profileSpec.And(tmp);
                    }
                    else
                    {
                        ISpecification<FacebookUserProfile> tmp = new InitialSpecification<FacebookUserProfile>(x => false);
                        foreach (var spec in itemSpecs)
                            tmp = tmp.Or(spec);
                        profileSpec = profileSpec.And(tmp);
                    }
                }
            }

            var userProfileObj = GetService<IFacebookUserProfileService>();
            return userProfileObj.SearchQuery(profileSpec.AsExpression()).ToList();
        }

        private Expression ToLamdaExpression(SimpleFilterItem item, ParameterExpression parameter)
        {
            Expression resultExpression = null;
            if (item.type == "Name" || item.type == "FirstName" || item.type == "LastName" || item.type == "Gender")
            {
                Expression left = Expression.PropertyOrField(parameter, item.type);
                Expression right = Expression.Constant(item.formula_value);
                switch (item.formula_type)
                {
                    case "contains":
                    case "doesnotcontain":
                    case "startswith":
                        var nullCheckExpression = Expression.Equal(left, Expression.Constant(null, typeof(String)));

                        if (item.formula_type == "contains" || item.formula_type == "doesnotcontain")
                        {
                            var containsMethod = typeof(String).GetMethod("Contains", new[] { typeof(String) });
                            var containsExpression = Expression.Call(left, containsMethod, right);
                            if (item.formula_type == "contains")
                                resultExpression = Expression.AndAlso(Expression.Not(nullCheckExpression), containsExpression);
                            else
                                resultExpression = Expression.AndAlso(Expression.Not(nullCheckExpression), Expression.Not(containsExpression));
                        }
                        else if (item.formula_type == "startswith")
                        {
                            var startswithMethod = typeof(String).GetMethod("StartsWith", new[] { typeof(String) });
                            var startswithExpression = Expression.Call(left, startswithMethod, right);
                            resultExpression = Expression.AndAlso(Expression.Not(nullCheckExpression), startswithExpression);
                        }
                        break;
                    case "eq":
                    case "neq":
                        var equalCheckExpression = Expression.Equal(left, right);
                        if (item.formula_type == "eq")
                            resultExpression = equalCheckExpression;
                        else
                            resultExpression = Expression.Not(equalCheckExpression);
                        break;
                    default:
                        throw new NotSupportedException(string.Format("Not support Operator {0}!", item.formula_type));
                }
            }
            else if (item.type == "Tag")
            {
                Expression tagRelsExpression = Expression.PropertyOrField(parameter, "TagRels");
                switch (item.formula_type)
                {
                    case "eq":
                    case "neq":
                        // find Any method
                        var containsMethod = typeof(ICollection<FacebookUserProfileTagRel>).GetMethods()
                            .Where(m => m.Name == "Any")
                            .Single(m => m.GetParameters().Length == 2);

                        var tagRelParameter = Expression.Parameter(typeof(FacebookUserProfileTagRel), "s");

                        Expression left = Expression.PropertyOrField(tagRelParameter, "Name");
                        Expression right = Expression.Constant(item.formula_value);
                        Expression equalExpression = Expression.Equal(left, right);

                        var containsExpression = Expression.Call(tagRelsExpression, containsMethod, equalExpression);
                        if (item.formula_type == "eq")
                            resultExpression = containsExpression;
                        else
                            resultExpression = Expression.Not(containsExpression);
                        break;
                    default:
                        throw new NotSupportedException(string.Format("Not support Operator {0}!", item.formula_type));
                }
            }
            else
                throw new NotSupportedException(string.Format("Not support type {0}!", item.type));

            return resultExpression;
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

        public async Task<PagedResult2<FacebookUserProfileBasic>> ActionStatistics(Guid id, FacebookMassMessagingStatisticsPaged paged)
        {
            var traceObj = GetService<IFacebookMessagingTraceService>();
            var query = traceObj.SearchQuery(x => x.MassMessagingId == id);

            if (paged.Type == "delivered")
                query = query.Where(x => x.Delivered.HasValue);
            else if (paged.Type == "opened")
                query = query.Where(x => x.Opened.HasValue);
            else if (paged.Type == "sent")
                query = query.Where(x => x.Sent.HasValue);

            var items = await query.OrderBy(x => x.DateCreated).Skip(paged.Offset).Take(paged.Limit).Select(x => new FacebookUserProfileBasic
            {
                Id = x.UserProfile.Id,
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

        public async Task TagStatistics(FacebookMassMessagingTagStatistics val)
        {
            var profileObj = GetService<IFacebookUserProfileService>();
            var limit = 200;
            var offset = 0;

            var pagedResult = await ActionStatistics(val.Id, 
                new FacebookMassMessagingStatisticsPaged { Offset = offset, Limit = limit, Type = val.Type });
            while(pagedResult.Items.Count() > 0)
            {
                var ids = pagedResult.Items.Select(x => x.Id).ToList();
                var profiles = await profileObj.SearchQuery(x => ids.Contains(x.Id)).Include(x => x.TagRels).ToListAsync();
                foreach(var profile in profiles)
                {
                    foreach(var tagId in val.TagIds)
                    {
                        if (!profile.TagRels.Any(x => x.TagId == tagId))
                            profile.TagRels.Add(new FacebookUserProfileTagRel { TagId = tagId });
                    }
                }

                await profileObj.UpdateAsync(profiles);
                offset += limit;

                pagedResult = await ActionStatistics(val.Id,
                new FacebookMassMessagingStatisticsPaged { Offset = offset, Limit = limit, Type = val.Type });
            }
        }
    }
}
