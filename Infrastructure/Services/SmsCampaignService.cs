using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
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
    public class SmsCampaignService : BaseService<SmsCampaign>, ISmsCampaignService
    {
        private readonly IMapper _mapper;
        public SmsCampaignService(IMapper mapper, IAsyncRepository<SmsCampaign> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        private IQueryable<SmsCampaign> GetQueryable(SmsCampaignPaged val)
        {
            var query = SearchQuery(x => !string.IsNullOrEmpty(x.TypeDate));
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));
            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State == val.State);


            return query;
        }

        public async Task<PagedResult2<SmsCampaignBasic>> GetPaged(SmsCampaignPaged val)
        {
            var query = GetQueryable(val);
            var smsMessageObj = GetService<ISmsMessageService>();
            var smsMessageDetailObj = GetService<ISmsMessageDetailService>();
            var dictTotalSuccessfulMessage = new Dictionary<Guid, int>();
            var dictTotalFailedMessage = new Dictionary<Guid, int>();

            var ditcTotalWaitedMessage = await smsMessageObj
                .SearchQuery(x => x.SmsCampaignId.HasValue && x.State == "waiting")
                .Include(x => x.Partners)
                .ToDictionaryAsync(x => x.SmsCampaignId.Value, x => x.Partners.Count());

            var listSuccess = await smsMessageDetailObj.SearchQuery(x => x.SmsCampaignId.HasValue && x.State == "success").ToListAsync();
            if (listSuccess.Any())
            {
                dictTotalSuccessfulMessage = listSuccess.GroupBy(x => x.SmsCampaignId.Value).ToDictionary(x => x.Key, x => x.Count());
            }

            var listFails = await smsMessageDetailObj.SearchQuery(x => x.SmsCampaignId.HasValue && x.State == "fails").ToListAsync();
            if (listFails.Any())
            {
                dictTotalFailedMessage = listFails.GroupBy(x => x.SmsCampaignId.Value).ToDictionary(x => x.Key, x => x.Count());
            }

            var totalItems = await query.CountAsync();
            var items = await query.Skip(val.Offset).Take(val.Limit).Select(x => new SmsCampaignBasic
            {
                Id = x.Id,
                Name = x.Name,
                DateEnd = x.DateEnd,
                DateStart = x.DateStart,
                LimitMessage = x.LimitMessage,
                State = x.State,
                TypeDate = x.TypeDate,
                TotalWaitedMessages = ditcTotalWaitedMessage.ContainsKey(x.Id) ? ditcTotalWaitedMessage[x.Id] : 0,
                TotalFailedMessages = dictTotalFailedMessage.ContainsKey(x.Id) ? dictTotalFailedMessage[x.Id] : 0,
                TotalSuccessfulMessages = dictTotalSuccessfulMessage.ContainsKey(x.Id) ? dictTotalSuccessfulMessage[x.Id] : 0,
                TotalMessage =
                (ditcTotalWaitedMessage.ContainsKey(x.Id) ? ditcTotalWaitedMessage[x.Id] : 0) +
                 (dictTotalFailedMessage.ContainsKey(x.Id) ? dictTotalFailedMessage[x.Id] : 0) +
                 (dictTotalSuccessfulMessage.ContainsKey(x.Id) ? dictTotalSuccessfulMessage[x.Id] : 0)
            }).ToListAsync();
            return new PagedResult2<SmsCampaignBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<SmsCampaign> GetDefaultCampaignBirthday()
        {
            var modelDataObj = GetService<IIRModelDataService>();
            var campaign = await modelDataObj.GetRef<SmsCampaign>("base.sms_campaign_birthday");
            if (campaign == null)
            {
                campaign = new SmsCampaign
                {
                    Name = "Chúc mừng sinh nhật",
                    CompanyId = CompanyId
                };

                await CreateAsync(campaign);

                await modelDataObj.CreateAsync(new IRModelData
                {
                    Name = "sms_campaign_birthday",
                    Module = "base",
                    Model = "res.sms.campaign",
                    ResId = campaign.Id.ToString()
                });
            }

            return campaign;
        }

        public async Task<SmsCampaign> GetDefaultCampaignAppointmentReminder()
        {
            var modelDataObj = GetService<IIRModelDataService>();
            var campaign = await modelDataObj.GetRef<SmsCampaign>("base.sms_campaign_appointment_reminder");
            if (campaign == null)
            {
                campaign = new SmsCampaign
                {
                    Name = "Nhắc lịch hẹn",
                    CompanyId = CompanyId
                };

                await CreateAsync(campaign);

                await modelDataObj.CreateAsync(new IRModelData
                {
                    Name = "sms_campaign_appointment_reminder",
                    Module = "base",
                    Model = "res.sms.campaign",
                    ResId = campaign.Id.ToString()
                });
            }

            return campaign;
        }

        public async Task<SmsCampaignBasic> CreateAsync(SmsCampaignSave val)
        {
            var entity = _mapper.Map<SmsCampaign>(val);
            entity.CompanyId = CompanyId;
            if (entity.TypeDate == "unlimited")
            {
                entity.State = "running";
            }
            else if (entity.TypeDate == "period")
            {
                if (entity.DateStart.HasValue &&
                    entity.DateStart.Value <= DateTime.Today &&
                    entity.DateEnd.HasValue &&
                    entity.DateEnd.Value >= DateTime.Today)
                {
                    entity.State = "running";
                }
                else if (entity.DateStart.HasValue && entity.DateStart.Value > DateTime.Today)
                {
                    entity.State = "draft";
                }
                else
                {
                    entity.State = "shutdown";
                }
            }
            entity = await CreateAsync(entity);
            return _mapper.Map<SmsCampaignBasic>(entity);
        }

        public async Task UpdateAsync(Guid id, SmsCampaignSave val)
        {
            var entity = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            entity = _mapper.Map(val, entity);
            if (entity.TypeDate == "period")
            {
                if (entity.DateStart.HasValue &&
                    entity.DateStart.Value <= DateTime.Today &&
                    entity.DateEnd.HasValue &&
                    entity.DateEnd.Value >= DateTime.Today)
                {
                    entity.State = "running";
                }
                else if (entity.DateEnd.HasValue && entity.DateEnd.Value <= DateTime.Today)
                {
                    entity.State = "draft";
                }
                else
                {
                    entity.State = "shutdown";
                }
            }
            await UpdateAsync(entity);
        }

    }
}
