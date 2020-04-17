using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class MarketingCampaignActivityService : BaseService<MarketingCampaignActivity>, IMarketingCampaignActivityService
    {
        private readonly IMapper _mapper;
        public MarketingCampaignActivityService(IAsyncRepository<MarketingCampaignActivity> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public void CheckAutoTakeCoupon(IEnumerable<MarketingCampaignActivity> self)
        {
            //if (self.Any(x => x.AutoTakeCoupon == true && !x.CouponProgramId.HasValue))
            //    throw new Exception("Vui lòng chọn 1 chương trình coupon");
            //foreach(var activity in self)
            //{
            //    if (activity.AutoTakeCoupon != true)
            //        continue;
            //    if (!activity.CouponProgramId.HasValue)
            //        throw new Exception("Vui lòng chọn 1 chương trình coupon");
            //    var ma_tron = "{ma_coupon}";
            //    if (!activity.Content.Contains($"{ma_tron}"))
            //        throw new Exception($"Bạn nên chèn mã trộn {ma_tron} vào nội dung gửi");
            //}
        }

        public async Task<PagedResult2<MarketingCampaignActivitySimple>> AutocompleteActivity(MarketingCampaignActivityPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));
            if (val.ParentId.HasValue)
            {
                query = SearchQuery().Where(x => x.Id == val.ParentId);
            }
            var items = await _mapper.ProjectTo<MarketingCampaignActivitySimple>(query).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<MarketingCampaignActivitySimple>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
           
        }

        public async Task<MarketingCampaignActivityDisplay> GetActivityDisplay(Guid id)
        {
            var activity = await SearchQuery(x => x.Id == id)
              .Include(x => x.Message).Include("Message.Buttons").Select(x=> new MarketingCampaignActivityDisplay {
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

              })
              .FirstOrDefaultAsync();

            return activity;
        }
        public async Task<MarketingCampaignActivity> CreateActivity(MarketingCampaignActivitySave val)
        {
            var activity = _mapper.Map<MarketingCampaignActivity>(val);
            var message = new MarketingMessage() { Template = val.Template };
            SaveMessage(message, val);
            activity.Message = message;
            await CreateAsync(activity);

            return activity;

        }



        public async Task UpdateActivity(Guid id, MarketingCampaignActivitySave val)
        {
            var activity = await SearchQuery(x => x.Id == id)
               .Include(x => x.Message).Include("Message.Buttons")
               .FirstOrDefaultAsync();
            if (activity == null)
                throw new ArgumentNullException("activity");
           
            activity = _mapper.Map(val, activity);
            var message = new MarketingMessage() { Template = val.Template };
            SaveMessage(message, val);
            activity.Message = message;


            await UpdateAsync(activity);
        }

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

        public async Task RemoveActivity(IEnumerable<Guid> ids)
        {
            var messageService = GetService<IMarketingMessageService>();
            var activities = await SearchQuery(x => ids.Contains(x.Id))
              .Include(x => x.Message).Include("Message.Buttons").ToListAsync();
            foreach(var activity in activities)
            {
                if (string.IsNullOrEmpty(activity.JobId))
                    continue;
                BackgroundJob.Delete(activity.JobId);
                if (activity.MessageId.HasValue)
                    await messageService.DeleteAsync(activity.Message);


            }

            await DeleteAsync(activities);
        }

      

    }
}
