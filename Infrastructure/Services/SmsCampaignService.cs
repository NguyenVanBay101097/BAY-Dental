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
            var query = SearchQuery(x => x.CompanyId == CompanyId);
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));
            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State == val.State);
            if (val.Combobox.HasValue && val.Combobox.Value)
                query = query.Where(x => string.IsNullOrEmpty(x.DefaultType) || x.DefaultType.Equals("sms_campaign_default"));

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
                .SearchQuery(x => x.SmsCampaignId.HasValue && x.State == "waiting" && x.CompanyId == CompanyId)
                .Include(x => x.SmsMessagePartnerRels)
                .ToDictionaryAsync(x => x.SmsCampaignId.Value, x => x.SmsMessagePartnerRels.Count());

            var listSuccess = await smsMessageDetailObj.SearchQuery().Where(x => x.SmsCampaignId.HasValue && x.State == "success" && x.CompanyId == CompanyId).ToListAsync();
            if (listSuccess.Any())
            {
                dictTotalSuccessfulMessage = listSuccess.GroupBy(x => x.SmsCampaignId.Value).ToDictionary(x => x.Key, x => x.Count());
            }

            var listFails = await smsMessageDetailObj.SearchQuery().Where(x => x.SmsCampaignId.HasValue && x.State == "fails" && x.CompanyId == CompanyId).ToListAsync();
            if (listFails.Any())
            {
                dictTotalFailedMessage = listFails.GroupBy(x => x.SmsCampaignId.Value).ToDictionary(x => x.Key, x => x.Count());
            }

            var totalItems = await query.CountAsync();
            var items = await query.Skip(val.Offset).Take(val.Limit).OrderByDescending(x => x.DateCreated).Select(x => new SmsCampaignBasic
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
            await UpdateAsync(entity);
        }

        public async Task<SmsCampaignBasic> GetDisplay(Guid id)
        {
            var smsMessageObj = GetService<ISmsMessageService>();
            var smsMessageDetailObj = GetService<ISmsMessageDetailService>();
            var TotalWait = await smsMessageObj.SearchQuery(x => x.SmsCampaignId.HasValue && x.SmsCampaignId.Value == id && x.State == "waiting" && x.CompanyId == CompanyId).SelectMany(x => x.SmsMessagePartnerRels).CountAsync();
            var TotalSuccess = await smsMessageDetailObj.SearchQuery().Where(x => x.SmsCampaignId.HasValue && x.SmsCampaignId.Value == id && x.State == "success" && x.CompanyId == CompanyId).CountAsync();
            var TotalFails = await smsMessageDetailObj.SearchQuery().Where(x => x.SmsCampaignId.HasValue && x.SmsCampaignId.Value == id && x.State == "fails" && x.CompanyId == CompanyId).CountAsync();

            var campaign = await SearchQuery(x => x.Id == id).Select(x => new SmsCampaignBasic
            {
                Id = x.Id,
                DateEnd = x.DateEnd,
                DateStart = x.DateStart,
                DefaultType = x.DefaultType,
                LimitMessage = x.LimitMessage,
                Name = x.Name,
                State = x.State,
                TotalMessage = TotalFails + TotalSuccess + TotalWait,
                TotalSuccessfulMessages = TotalSuccess,
                TotalFailedMessages = TotalFails,
                TotalWaitedMessages = TotalWait,
                TypeDate = x.TypeDate
            }).FirstOrDefaultAsync();
            return campaign;
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
                    CompanyId = CompanyId,
                    TypeDate = "unlimited",
                    State = "running",
                    DefaultType = "sms_campaign_birthday"
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
                    CompanyId = CompanyId,
                    TypeDate = "unlimited",
                    DefaultType = "sms_campaign_appointment_reminder",
                    State = "running"
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

        public async Task<SmsCampaign> GetDefaultCampaign()
        {
            var modelDataObj = GetService<IIRModelDataService>();
            var campaign = await modelDataObj.GetRef<SmsCampaign>("base.sms_campaign_default");
            if (campaign == null)
            {
                campaign = new SmsCampaign
                {
                    Name = "Chiến dịch mặc định",
                    CompanyId = CompanyId,
                    TypeDate = "unlimited",
                    State = "running",
                    DefaultType = "sms_campaign_default"
                };

                await CreateAsync(campaign);

                await modelDataObj.CreateAsync(new IRModelData
                {
                    Name = "sms_campaign_default",
                    Module = "base",
                    Model = "res.sms.campaign",
                    ResId = campaign.Id.ToString()
                });
            }

            return campaign;
        }



        public async Task<SmsCampaign> GetDefaultThanksCustomer()
        {
            var modelDataObj = GetService<IIRModelDataService>();
            var campaign = await modelDataObj.GetRef<SmsCampaign>("base.sms_campaign_thanks_customer");
            if (campaign == null)
            {
                campaign = new SmsCampaign
                {
                    Name = "Cảm ơn khách hàng",
                    CompanyId = CompanyId,
                    TypeDate = "unlimited",
                    State = "running",
                    DefaultType = "sms_campaign_thanks_customer"
                };

                await CreateAsync(campaign);

                await modelDataObj.CreateAsync(new IRModelData
                {
                    Name = "sms_campaign_thanks_customer",
                    Module = "base",
                    Model = "res.sms.campaign",
                    ResId = campaign.Id.ToString()
                });
            }
            return campaign;
        }

        public async Task<SmsCampaign> GetDefaultCareAfterOrder()
        {
            var modelDataObj = GetService<IIRModelDataService>();
            var campaign = await modelDataObj.GetRef<SmsCampaign>("base.sms_campaign_care_after_order");
            if (campaign == null)
            {
                campaign = new SmsCampaign
                {
                    Name = "Tin nhắn chăm sóc sau điều trị",
                    CompanyId = CompanyId,
                    TypeDate = "unlimited",
                    State = "running",
                    DefaultType = "sms_campaign_care_after_order"
                };

                await CreateAsync(campaign);

                await modelDataObj.CreateAsync(new IRModelData
                {
                    Name = "sms_campaign_care_after_order",
                    Module = "base",
                    Model = "res.sms.campaign",
                    ResId = campaign.Id.ToString()
                });
            }
            return campaign;
        }
    }
}
