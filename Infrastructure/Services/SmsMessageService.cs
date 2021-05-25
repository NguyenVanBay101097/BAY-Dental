using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class SmsMessageService : BaseService<SmsMessage>, ISmsMessageService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly AppTenant _tenant;
        public SmsMessageService(IConfiguration configuration, ITenant<AppTenant> tenant, IMapper mapper, IAsyncRepository<SmsMessage> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _tenant = tenant?.Value;
            _configuration = configuration;
        }

        private IQueryable<SmsMessage> GetQueryable(SmsMessagePaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));
            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State == val.State);
            if (val.CampaignId.HasValue)
                query = query.Where(x => x.SmsCampaignId.Value == val.CampaignId.Value);
            if (val.SmsAccountId.HasValue)
                query = query.Where(x => x.SmsAccountId.HasValue && x.SmsAccountId == val.SmsAccountId.Value);
            if (val.DateFrom.HasValue)
                query = query.Where(x => x.DateCreated.HasValue && val.DateFrom.Value <= x.Date.Value);
            if (val.DateTo.HasValue)
                query = query.Where(x => x.DateCreated.HasValue && val.DateTo.Value >= x.Date.Value);
            return query;
        }

        public async Task<PagedResult2<SmsMessageBasic>> GetPaged(SmsMessagePaged val)
        {
            var query = GetQueryable(val);
            var totalItems = await query.CountAsync();
            var items = await query.Skip(val.Offset).Take(val.Limit).Select(x => new SmsMessageBasic
            {
                Id = x.Id,
                Date = x.Date,
                DateCreated = x.DateCreated,
                Name = x.Name,
                CountPartner = x.SmsMessagePartnerRels.Count(),
                BrandName = x.SmsAccountId.HasValue ? x.SmsAccount.BrandName + $" ({x.SmsAccount.Name})" : "",
            }).ToListAsync();
            return new PagedResult2<SmsMessageBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<SmsMessageDisplay> CreateAsync(SmsMessageSave val)
        {
            var entity = _mapper.Map<SmsMessage>(val);
            if (entity.TypeSend == "manual")
                entity.State = "sending";
            else
            {
                entity.State = "waiting";
            }

            if (!entity.SmsCampaignId.HasValue && val.IsBirthDayManual.HasValue && val.IsBirthDayManual.Value)
            {
                var smsCampaignObj = GetService<ISmsCampaignService>();
                var campaign = await smsCampaignObj.GetDefaultCampaignBirthday();
                entity.SmsCampaignId = campaign.Id;
                entity.ResModel = "partner";
                if (val.GuidIds.Any())
                {
                    foreach (var id in val.GuidIds)
                    {
                        entity.SmsMessagePartnerRels.Add(new SmsMessagePartnerRel()
                        {
                            PartnerId = id
                        });
                    }
                }
            }

            else if (!entity.SmsCampaignId.HasValue && val.IsCareAfterOrder.HasValue && val.IsCareAfterOrder.Value)
            {
                var smsCampaignObj = GetService<ISmsCampaignService>();
                var campaign = await smsCampaignObj.GetDefaultCampaignAppointmentReminder();
                entity.SmsCampaignId = campaign.Id;
                entity.ResModel = "sale-order-line";
                if (val.GuidIds.Any())
                {
                    foreach (var id in val.GuidIds)
                    {
                        entity.SmsMessageSaleOrderLineRels.Add(new SmsMessageSaleOrderLineRel()
                        {
                            SaleOrderLineId = id
                        });
                    }
                }
            }

            else if (!entity.SmsCampaignId.HasValue && val.IsThanksCustomer.HasValue && val.IsThanksCustomer.Value)
            {
                var smsCampaignObj = GetService<ISmsCampaignService>();
                var campaign = await smsCampaignObj.GetDefaultThanksCustomer();
                entity.SmsCampaignId = campaign.Id;
                entity.ResModel = "sale-order";
                if (val.GuidIds.Any())
                {
                    foreach (var id in val.GuidIds)
                    {
                        entity.SmsMessageSaleOrderRels.Add(new SmsMessageSaleOrderRel()
                        {
                            SaleOrderId = id
                        });
                    }
                }
            }

            else if (!entity.SmsCampaignId.HasValue && val.IsThanksCustomer.HasValue && val.IsThanksCustomer.Value)
            {
                var smsCampaignObj = GetService<ISmsCampaignService>();
                var campaign = await smsCampaignObj.GetDefaultThanksCustomer();
                entity.SmsCampaignId = campaign.Id;
                entity.ResModel = "sale-order";
                if (val.GuidIds.Any())
                {
                    foreach (var id in val.GuidIds)
                    {
                        entity.SmsMessageSaleOrderRels.Add(new SmsMessageSaleOrderRel()
                        {
                            SaleOrderId = id
                        });
                    }
                }
            }

            else if (!entity.SmsCampaignId.HasValue)
            {
                var smsCampaignObj = GetService<ISmsCampaignService>();
                var campaign = await smsCampaignObj.GetDefaultCampaign();
                entity.SmsCampaignId = campaign.Id;
                entity.ResModel = "partner";
                if (val.GuidIds.Any())
                {
                    foreach (var id in val.GuidIds)
                    {
                        entity.SmsMessagePartnerRels.Add(new SmsMessagePartnerRel()
                        {
                            PartnerId = id
                        });
                    }
                }
            }

            entity = await CreateAsync(entity);
            return _mapper.Map<SmsMessageDisplay>(entity);
        }

        public async Task ActionSendSMS(SmsMessage entity)
        {
            var smsSendMessageObj = GetService<ISmsSendMessageService>();
            var hostName = _tenant != null ? _tenant.Hostname : "localhost";
            await using var context = DbContextHelper.GetCatalogDbContext(hostName, _configuration);
            try
            {
                var partnerIds = new List<Guid>();
                switch (entity.ResModel)
                {
                    case "partner":
                        partnerIds = entity.SmsMessagePartnerRels.Select(x => x.PartnerId).ToList();
                        break;
                    case "appointment":

                        break;
                    case "sale-order":
                        break;
                    case "sale-order-line":
                        break;
                    default:
                        break;
                }


                if (partnerIds.Any())
                {
                    var companyId = CompanyId;
                    await smsSendMessageObj.CreateSmsMessageDetail(context, entity, partnerIds, companyId);
                    entity.State = "success";
                }
                await UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //private async Task<string> PersonalizedContent(string body)
        //{
        //    var content = JsonConvert.DeserializeObject<Body>(body);
        //    var messageContent = content.text
        //        .Replace("{ten_khach_hang}", partner.Name.Split(' ').Last())
        //        .Replace("{ho_ten_khach_hang}", partner.DisplayName)
        //        .Replace("{ten_cong_ty}", company.Name)
        //        .Replace("{ngay_sinh}", partner.Name)
        //        .Replace("{gio_hen}", partner.Name)
        //        .Replace("{ngay_hen}", partner.Name)
        //        .Replace("{bac_si_lich_hen}", partner.Name)
        //        .Replace("{bac_si_chi_tiet_dieu_tri}", partner.Name)
        //        .Replace("{so_phieu_dieu_tri}", partner.Name)
        //        .Replace("{dich_vu}", partner.Name)
        //        .Replace("{danh_xung_khach_hang}", partner.Title != null ? partner.Title.Name : "");
        //    return messageContent;
        //}


        public async Task ActionCancel(IEnumerable<Guid> messIds)
        {
            var messes = await SearchQuery(x => messIds.Contains(x.Id)).ToListAsync();
            if (messes.Any())
            {
                await DeleteAsync(messes);
            }
        }

        public class Body
        {
            public string templateType { get; set; }
            public string text { get; set; }
        }
    }
}
