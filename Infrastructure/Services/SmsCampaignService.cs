﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
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
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));
            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State == val.State);
            if (val.CompanyId.HasValue)
                query = query.Where(x => x.CompanyId.HasValue || x.CompanyId == val.CompanyId);
            if (val.UserCampaign.HasValue)
                query = query.Where(x => x.UserCampaign == val.UserCampaign);

            return query;
        }

        public async Task<PagedResult2<SmsCampaignBasic>> GetPaged(SmsCampaignPaged val)
        {
            var query = GetQueryable(val);
            var smsMessageObj = GetService<ISmsMessageService>();
            var smsMessageDetailObj = GetService<ISmsMessageDetailService>();
            var dictTotalSuccessfulMessage = new Dictionary<Guid, int>();
            var dictTotalFailedMessage = new Dictionary<Guid, int>();
            var dictTotalCanceledMessage = new Dictionary<Guid, int>();
            var dictTotalOutgoingMessage = new Dictionary<Guid, int>();

            //var ditcTotalWaitedMessage = await smsMessageObj
            //    .SearchQuery().Where(x => x.SmsCampaignId.HasValue && x.State == "in_queue" && x.CompanyId == CompanyId)
            //    .Include(x => x.SmsMessagePartnerRels)
            //    .ToDictionaryAsync(x => x.SmsCampaignId.Value, x => x.SmsMessagePartnerRels.Count());

            //var listSuccess = await smsMessageDetailObj.SearchQuery().Where(x => x.SmsCampaignId.HasValue && x.State == "sent" && x.CompanyId == CompanyId).ToListAsync();
            //if (listSuccess.Any())
            //{
            //    dictTotalSuccessfulMessage = listSuccess.GroupBy(x => x.SmsCampaignId.Value).ToDictionary(x => x.Key, x => x.Count());
            //}

            //var listFails = await smsMessageDetailObj.SearchQuery().Where(x => x.SmsCampaignId.HasValue && x.State == "error" && x.CompanyId == CompanyId).ToListAsync();
            //if (listFails.Any())
            //{
            //    dictTotalFailedMessage = listFails.GroupBy(x => x.SmsCampaignId.Value).ToDictionary(x => x.Key, x => x.Count());
            //}

            //var listCancel = await smsMessageDetailObj.SearchQuery().Where(x => x.SmsCampaignId.HasValue && x.State == "canceled" && x.CompanyId == CompanyId).ToListAsync();
            //if (listCancel.Any())
            //{
            //    dictTotalCanceledMessage = listCancel.GroupBy(x => x.SmsCampaignId.Value).ToDictionary(x => x.Key, x => x.Count());
            //}

            //var listOutgoing = await smsMessageDetailObj.SearchQuery().Where(x => x.SmsCampaignId.HasValue && x.State == "outgoing" && x.CompanyId == CompanyId).ToListAsync();
            //if (listOutgoing.Any())
            //{
            //    dictTotalOutgoingMessage = listOutgoing.GroupBy(x => x.SmsCampaignId.Value).ToDictionary(x => x.Key, x => x.Count());
            //}

            var totalItems = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.DateCreated).Skip(val.Offset).Take(val.Limit).Select(x => new SmsCampaignBasic
            {
                Id = x.Id,
                Name = x.Name,
                DateEnd = x.DateEnd,
                DateStart = x.DateStart,
                LimitMessage = x.LimitMessage,
                State = x.State,
                TypeDate = x.TypeDate,
                //TotalWaitedMessages = ditcTotalWaitedMessage.ContainsKey(x.Id) ? ditcTotalWaitedMessage[x.Id] : 0,
                //TotalCancelMessages = dictTotalCanceledMessage.ContainsKey(x.Id) ? dictTotalCanceledMessage[x.Id] : 0,
                //TotaOutgoingMessages = dictTotalOutgoingMessage.ContainsKey(x.Id) ? dictTotalOutgoingMessage[x.Id] : 0,
                //TotalErrorMessages = dictTotalFailedMessage.ContainsKey(x.Id) ? dictTotalFailedMessage[x.Id] : 0,
                //TotalSuccessfulMessages = dictTotalSuccessfulMessage.ContainsKey(x.Id) ? dictTotalSuccessfulMessage[x.Id] : 0,
                //TotalMessage =
                //  (ditcTotalWaitedMessage.ContainsKey(x.Id) ? ditcTotalWaitedMessage[x.Id] : 0) +
                //  (dictTotalCanceledMessage.ContainsKey(x.Id) ? dictTotalCanceledMessage[x.Id] : 0) +
                //  (dictTotalOutgoingMessage.ContainsKey(x.Id) ? dictTotalOutgoingMessage[x.Id] : 0) +
                //   (dictTotalFailedMessage.ContainsKey(x.Id) ? dictTotalFailedMessage[x.Id] : 0) +
                //   (dictTotalSuccessfulMessage.ContainsKey(x.Id) ? dictTotalSuccessfulMessage[x.Id] : 0)
            }).ToListAsync();

            var ids = items.Select(x => x.Id).ToList();
            var statistics = await smsMessageDetailObj.SearchQuery().Where(x => x.SmsCampaignId.HasValue && ids.Contains(x.SmsCampaignId.Value))
                .GroupBy(x => x.SmsCampaignId.Value)
                .Select(x => new
                {
                    CampaignId = x.Key,
                    Total = x.Sum(s => 1),
                    TotalSent = x.Sum(s => s.State == "sent" ? 1 : 0),
                    TotalError = x.Sum(s => s.State == "error" ? 1 : 0),
                }).ToListAsync();
            var statistics_dict = statistics.ToDictionary(x => x.CampaignId, x => x);

            foreach (var item in items)
            {
                if (!statistics_dict.ContainsKey(item.Id))
                    continue;
                var statistic = statistics_dict[item.Id];
                item.TotalMessage = statistic.Total;
                item.TotalSuccessfulMessages = statistic.TotalSent;
                item.TotalErrorMessages = statistic.TotalError;
            }

            return new PagedResult2<SmsCampaignBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<SmsCampaignBasic> CreateAsync(SmsCampaignSave val)
        {
            var entity = _mapper.Map<SmsCampaign>(val);
            entity.CompanyId = CompanyId;
            //if (entity.TypeDate == "unlimited")
            //{
            //    entity.State = "running";
            //}
            //else if (entity.TypeDate == "period")
            //{
            //    if (entity.DateStart.HasValue &&
            //        entity.DateStart.Value <= DateTime.Today &&
            //        entity.DateEnd.HasValue &&
            //        entity.DateEnd.Value >= DateTime.Today)
            //    {
            //        entity.State = "running";
            //    }
            //    else if (entity.DateStart.HasValue && entity.DateStart.Value > DateTime.Today)
            //    {
            //        entity.State = "draft";
            //    }
            //    else
            //    {
            //        entity.State = "shutdown";
            //    }
            //}
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
            var TotalWait = await smsMessageObj.SearchQuery().Where(x => x.SmsCampaignId.HasValue && x.SmsCampaignId.Value == id && x.State == "waiting" && x.CompanyId == CompanyId).SelectMany(x => x.SmsMessagePartnerRels).CountAsync();
            var TotalSent = await smsMessageDetailObj.SearchQuery().Where(x => x.SmsCampaignId.HasValue && x.SmsCampaignId.Value == id && x.State == "sent" && x.CompanyId == CompanyId).CountAsync();
            var TotalError = await smsMessageDetailObj.SearchQuery().Where(x => x.SmsCampaignId.HasValue && x.SmsCampaignId.Value == id && x.State == "error" && x.CompanyId == CompanyId).CountAsync();
            var TotalCanceled = await smsMessageDetailObj.SearchQuery().Where(x => x.SmsCampaignId.HasValue && x.SmsCampaignId.Value == id && x.State == "canceled" && x.CompanyId == CompanyId).CountAsync();
            var TotalOutgoing = await smsMessageDetailObj.SearchQuery().Where(x => x.SmsCampaignId.HasValue && x.SmsCampaignId.Value == id && x.State == "outgoing" && x.CompanyId == CompanyId).CountAsync();

            var campaign = await SearchQuery(x => x.Id == id).Select(x => new SmsCampaignBasic
            {
                Id = x.Id,
                DateEnd = x.DateEnd,
                DateStart = x.DateStart,
                DefaultType = x.DefaultType,
                LimitMessage = x.LimitMessage,
                Name = x.Name,
                State = x.State,
                TotalMessage = TotalCanceled + TotalError + TotalOutgoing + TotalSent + TotalWait,
                TotalSuccessfulMessages = TotalSent,
                TotalCancelMessages = TotalCanceled,
                TotalErrorMessages = TotalError,
                TotaOutgoingMessages = TotalOutgoing,
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
                    LimitMessage = 0,
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
            else if (!campaign.CompanyId.HasValue)
            {
                campaign.CompanyId = CompanyId;
                await UpdateAsync(campaign);
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
                    LimitMessage = 0,
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
            else if (!campaign.CompanyId.HasValue)
            {
                campaign.CompanyId = CompanyId;
                await UpdateAsync(campaign);
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
                    LimitMessage = 0,
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
            else if (!campaign.CompanyId.HasValue)
            {
                campaign.CompanyId = CompanyId;
                await UpdateAsync(campaign);
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
                    LimitMessage = 0,
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
            else if (!campaign.CompanyId.HasValue)
            {
                campaign.CompanyId = CompanyId;
                await UpdateAsync(campaign);
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
                    Name = "Chăm sóc sau điều trị",
                    CompanyId = CompanyId,
                    LimitMessage = 0,
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
            else if (!campaign.CompanyId.HasValue)
            {
                campaign.CompanyId = CompanyId;
                await UpdateAsync(campaign);
            }
            return campaign;
        }

        public override ISpecification<SmsCampaign> RuleDomainGet(IRRule rule)
        {
            switch (rule.Code)
            {
                case "sms.sms_campaign_comp_rule":
                    return new InitialSpecification<SmsCampaign>(x => !x.CompanyId.HasValue || x.CompanyId == CompanyId);
                default:
                    return null;
            }
        }
    }
}
