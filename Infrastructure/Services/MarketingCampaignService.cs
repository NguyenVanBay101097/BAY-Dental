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

        public async Task<MarketingCampaign> CreateCampaign(MarketingCampaignSave val)
        {
            var campaign = _mapper.Map<MarketingCampaign>(val);
            // SaveActivities(val, campaign);

            await _EnsureFacebookPage(campaign);

            await CreateAsync(campaign);

            return campaign;
        }

        private async Task _EnsureFacebookPage(MarketingCampaign campaign)
        {
            if (!campaign.FacebookPageId.HasValue)
            {
                var userObj = GetService<IUserService>();
                var user = await userObj.GetCurrentUser();
                if (!user.FacebookPageId.HasValue)
                    throw new Exception("You must select a facebook page");
                campaign.FacebookPageId = user.FacebookPageId;
            }
        }

        public async Task UpdateCampaign(Guid id, MarketingCampaignSave val)
        {
            var campaign = await SearchQuery(x => x.Id == id)
                .Include(x => x.Activities).Include("Activities.Message")
                .Include("Activities.Message.Buttons").FirstOrDefaultAsync();
            if (campaign == null)
                throw new ArgumentNullException("campaign");

            campaign = _mapper.Map(val, campaign);
            // SaveActivities(val, campaign);

            await _EnsureFacebookPage(campaign);

            await UpdateAsync(campaign);
        }

        //private void SaveActivities(MarketingCampaignSave val, MarketingCampaign campaign)
        //{
        //    var Obj = GetService<IMarketingCampaignActivityService>();
        //    var existLines = campaign.Activities.ToList();
        //    var lineToRemoves = new List<MarketingCampaignActivity>();
        //    foreach (var existLine in existLines)
        //    {
        //        bool found = false;
        //        foreach (var item in val.Activities)
        //        {
        //            if (item.Id == existLine.Id)
        //            {
        //                found = true;
        //                break;
        //            }
        //        }

        //        if (!found)
        //            lineToRemoves.Add(existLine);
        //    }

        //    foreach (var line in lineToRemoves)
        //        campaign.Activities.Remove(line);

        //    int sequence = 0;

        //    foreach (var line in val.Activities)
        //    {

        //        if (line.Id == Guid.Empty)
        //        {
        //            var act = _mapper.Map<MarketingCampaignActivity>(line);
        //            act.Sequence = sequence++;

        //            var message = new MarketingMessage() { Template = line.Template };
        //            SaveMessage(message, line);

        //            act.Message = message;
        //            campaign.Activities.Add(act);
        //            if (act.ActivityChilds.Any())
        //            {
        //                foreach (var item in act.ActivityChilds)
        //                {
        //                    item.Sequence = sequence++;
        //                    var messageitem = new MarketingMessage() { Template = line.ActivityChilds.SingleOrDefault(x => x.Id == item.Id).Template };
        //                    SaveMessage(messageitem, line.ActivityChilds.SingleOrDefault(x => x.Id == item.Id));
        //                    item.Message = messageitem;
        //                    campaign.Activities.Add(item);
        //                }
        //            }


        //        }
        //        else
        //        {
        //            var activity = campaign.Activities.SingleOrDefault(c => c.Id == line.Id);
        //            if (activity != null)
        //            {
        //                _mapper.Map(line, activity);
        //                if (activity.ActivityChilds.Any())
        //                {
        //                    foreach (var item in activity.ActivityChilds)
        //                    {

        //                        item.Sequence = sequence++;
        //                        var messageitem = new MarketingMessage() { Template = line.ActivityChilds.SingleOrDefault(x => x.Id == item.Id).Template };
        //                        SaveMessage(messageitem, line.ActivityChilds.SingleOrDefault(x => x.Id == item.Id));
        //                        item.Message = messageitem;
        //                        campaign.Activities.Add(item);
        //                    }
        //                }

        //                if (activity.Message != null)
        //                {
        //                    var message = activity.Message;
        //                    SaveMessage(message, line);
        //                }
        //                activity.Sequence = sequence++;
        //            }
        //        }
        //    }
        //}

        public void SaveMessage(MarketingMessage message, MarketingCampaignActivitySave val)
        {
            if (message.Template == "text")
                message.Text = val.Text;
            else if (message.Template == "button")
            {
                message.Text = val.Text;
                message.Buttons.Clear();
                foreach (var button in val.Buttons)
                    message.Buttons.Add(_mapper.Map<MarketingMessageButton>(button));
            }
        }

        public async Task ActionStartCampaign(IEnumerable<Guid> ids)
        {
            var jobService = GetService<IMarketingCampaignActivityService>();
            var states = new string[] { "draft", "stopped" };
            var self = await SearchQuery(x => ids.Contains(x.Id) && states.Contains(x.State)).Include(x => x.Activities).ToListAsync();

            foreach (var campaign in self)
            {
                campaign.State = "running";
                campaign.DateStart = DateTime.Now;

                var parentAtivities = campaign.Activities.Where(x => x.ParentId == null).ToList();
                
                foreach (var activity in parentAtivities)
                {
                    var date = DateTime.UtcNow;
                    var jobId = "";
                    var datetime = new DateTime();
                    var childs = GetChildren(campaign.Activities.ToList(),activity.Id);
                    if (activity.TriggerType == "begin")
                    {
                        date = DateTime.UtcNow;
                        var intervalNumber = activity.IntervalNumber ?? 0;
                        if (activity.IntervalType == "hours")
                            date = date.AddHours(intervalNumber);
                        else if (activity.IntervalType == "minutes")
                            date = date.AddMinutes(intervalNumber);
                        else if (activity.IntervalType == "days")
                            date = date.AddDays(intervalNumber);
                        else if (activity.IntervalType == "months")
                            date = date.AddMonths(intervalNumber);
                        else if (activity.IntervalType == "weeks")
                            date = date.AddDays(intervalNumber * 7);
                        jobId = BackgroundJob.Schedule(() => _activityJobService.RunActivity(_tenant != null ? _tenant.Hostname : "localhost", activity.Id), date);
                        datetime = date;
                        activity.JobId = jobId;
                        if (string.IsNullOrEmpty(activity.JobId))
                            throw new Exception("Can't not schedule job");
                        await jobService.UpdateAsync(activity);
                        if (!childs.Any())
                        {
                            continue;
                        }
                        else
                        {

                            foreach (var child in childs)
                            {
                                if (child.TriggerType == "act" || child.TriggerType == "message_open")
                                {
                                    date = datetime;
                                    intervalNumber = child.IntervalNumber ?? 0;
                                    if (child.IntervalType == "hours")
                                        date = date.AddHours(intervalNumber);
                                    else if (child.IntervalType == "minutes")
                                        date = date.AddMinutes(intervalNumber);
                                    else if (child.IntervalType == "days")
                                        date = date.AddDays(intervalNumber);
                                    else if (child.IntervalType == "months")
                                        date = date.AddMonths(intervalNumber);
                                    else if (child.IntervalType == "weeks")
                                        date = date.AddDays(intervalNumber * 7);
                                    jobId = BackgroundJob.Schedule(() => _activityJobService.RunActivity(_tenant != null ? _tenant.Hostname : "localhost", child.Id), date);

                                    child.JobId = jobId;
                                    datetime = date;
                                    jobId = child.JobId;
                                    if (string.IsNullOrEmpty(activity.JobId))
                                        throw new Exception("Can't not schedule job");
                                    await jobService.UpdateAsync(child);

                                }


                            }
                        }
                    }




                }



            }

            await UpdateAsync(self);
        }

       

        /// <summary>
        /// GetAll child in parentId
        /// </summary>
        /// <param name="activities"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public List<MarketingCampaignActivity> GetChildren(List<MarketingCampaignActivity> activities, Guid parentId)
        {

            var lstactivity = activities
        .Where(x => x.ParentId == parentId)
        .Union(activities.Where(x => x.ParentId == parentId)
            .SelectMany(y => GetChildren(activities, y.Id))
        ).ToList();


            return lstactivity;
        }


        public async Task<MarketingCampaignDisplay> GetDisplay(Guid id)
        {

            var res = await SearchQuery(x => x.Id == id).Select(x => new MarketingCampaignDisplay
            {
                Id = x.Id,
                Name = x.Name,
                DateStart = x.DateStart,
                State = x.State,
            }).FirstOrDefaultAsync();

            var activityObj = GetService<IMarketingCampaignActivityService>();
            var activities = await activityObj.SearchQuery(x => x.CampaignId == id)
                .Select(x => new MarketingCampaignActivityDisplay
                {
                    Id = x.Id,
                    ActivityType = x.ActivityType,
                    Content = x.Content,
                    IntervalNumber = x.IntervalNumber,
                    IntervalType = x.IntervalType,
                    Name = x.Name,
                    TotalSent = x.Traces.Where(x => x.Sent.HasValue).Count(),
                    TotalRead = x.Traces.Where(x => x.Read.HasValue).Count(),
                    TotalDelivery = x.Traces.Where(x => x.Delivery.HasValue).Count(),
                    Template = x.Message.Template,
                    Text = x.Message.Text,
                    ParentId = x.ParentId,
                    Buttons = x.Message.Buttons.Select(s => new MarketingMessageButtonDisplay
                    {
                        Payload = s.Payload,
                        Title = s.Title,
                        Type = s.Type,
                        Url = s.Url
                    })
                }).ToListAsync();

            res.Activities = activities;
            return res;
        }

        public async Task ActionStopCampaign(IEnumerable<Guid> ids)
        {
            var states = new string[] { "running" };
            var self = await SearchQuery(x => ids.Contains(x.Id) && states.Contains(x.State)).Include(x => x.Activities).Include("Activities.Traces").ToListAsync();
            foreach (var campaign in self)
            {
                campaign.State = "stopped";
                campaign.DateStart = null;
                foreach (var activity in campaign.Activities)
                {
                    if (string.IsNullOrEmpty(activity.JobId))
                        continue;
                    activity.Traces.Clear();
                    BackgroundJob.Delete(activity.JobId);
                }
            }

            await UpdateAsync(self);
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Activities).ToListAsync();
            foreach (var campaign in self)
            {
                foreach (var activity in campaign.Activities)
                {
                    if (string.IsNullOrEmpty(activity.JobId))
                        continue;
                    BackgroundJob.Delete(activity.JobId);
                }
            }

            await DeleteAsync(self);
        }

        //---Activity----//

        public async Task<IEnumerable<MarketingCampaignActivitySimple>> AutocompleteActivity(Guid id)
        {
            var activityObj = GetService<IMarketingCampaignActivityService>();
            var activities = await activityObj.SearchQuery(x => x.CampaignId == id, orderBy: x => x.OrderBy(s => s.Sequence))
              .Select(x => new MarketingCampaignActivitySimple
              {
                  Id = x.Id,
                  Name = x.Name

              }).ToListAsync();
            return activities;
        }



    }


}
