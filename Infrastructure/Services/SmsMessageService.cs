using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Data;
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
        private readonly CatalogDbContext _context;
        private readonly AppTenant _tenant;
        public SmsMessageService(CatalogDbContext context, IConfiguration configuration, ITenant<AppTenant> tenant, IMapper mapper, IAsyncRepository<SmsMessage> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _tenant = tenant?.Value;
            _configuration = configuration;
            _context = context;
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
                var campaign = await smsCampaignObj.GetDefaultCareAfterOrder();
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

            else if (!entity.SmsCampaignId.HasValue && val.IsAppointmentReminder.HasValue && val.IsAppointmentReminder.Value)
            {
                var smsCampaignObj = GetService<ISmsCampaignService>();
                var campaign = await smsCampaignObj.GetDefaultCampaignAppointmentReminder();
                entity.SmsCampaignId = campaign.Id;
                entity.ResModel = "appointment";
                if (val.GuidIds.Any())
                {
                    foreach (var id in val.GuidIds)
                    {
                        entity.SmsMessageAppointmentRels.Add(new SmsMessageAppointmentRel()
                        {
                            AppointmentId = id
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

            else
            {
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

        public async Task ActionSendSMSMessage(SmsMessage entity)
        {
            var smsMessageDetailObj = GetService<ISmsMessageDetailService>();
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
                        var appointmentIds = entity.SmsMessageAppointmentRels.Select(x => x.AppointmentId);
                        var appObj = GetService<IAppointmentService>();
                        partnerIds = await appObj.SearchQuery(x => appointmentIds.Contains(x.Id)).Select(x => x.PartnerId).ToListAsync();
                        break;
                    case "sale-order":
                        var saleOrderObj = GetService<ISaleOrderService>();
                        var orderIds = entity.SmsMessageSaleOrderRels.Select(s => s.SaleOrderId).ToList();
                        partnerIds = await saleOrderObj.SearchQuery(x => orderIds.Contains(x.Id)).Select(x => x.PartnerId).ToListAsync();
                        break;
                    case "sale-order-line":
                        var saleOrderLineObj = GetService<ISaleOrderLineService>();
                        var orderLineIds = entity.SmsMessageSaleOrderLineRels.Select(s => s.SaleOrderLineId);
                        partnerIds = await saleOrderLineObj.SearchQuery(x => x.OrderPartnerId.HasValue && orderLineIds.Contains(x.Id)).Select(x => x.OrderPartnerId.Value).ToListAsync();
                        break;
                    default:
                        break;
                }


                if (partnerIds.Any())
                {
                    var companyId = CompanyId;
                    await smsMessageDetailObj.CreateSmsMessageDetail(entity, partnerIds, companyId);
                    entity.State = "success";
                }
                await UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task ActionCancel(IEnumerable<Guid> messIds)
        {
            var messes = await SearchQuery(x => messIds.Contains(x.Id)).ToListAsync();
            if (messes.Any())
            {
                await DeleteAsync(messes);
            }
        }

        public async Task SetupSendSmsOrderAutomatic(Guid orderId)
        {
            var configObj = GetService<ISmsConfigService>();
            var saleOrderObj = GetService<ISaleOrderService>();
            var saleOrder = await saleOrderObj.SearchQuery(x => x.Id == orderId).FirstOrDefaultAsync();
            var config = await configObj.SearchQuery(x => x.Type == "thanks-customer").FirstOrDefaultAsync();
            if (config != null && config.IsThanksCustomerAutomation && saleOrder.State == "done" && saleOrder.DateDone.HasValue)
            {
                var entity = new SmsMessage();
                var smsCampaignObj = GetService<ISmsCampaignService>();
                var campaign = await smsCampaignObj.GetDefaultThanksCustomer();
                entity.SmsCampaignId = campaign.Id;
                entity.ResModel = "sale-order";
                entity.Body = config.Body;
                entity.Date = config.TypeTimeBeforSend == "hour" ? saleOrder.DateDone.Value.AddHours(config.TimeBeforSend) : saleOrder.DateDone.Value.AddDays(config.TimeBeforSend);
                entity.SmsAccountId = config.SmsAccountId;
                entity.SmsTemplateId = config.TemplateId;
                entity.State = "waiting";
                entity.TypeSend = "automatic";
                entity.Name = $"Tin nhắn cảm ơn khách hàng ngày {DateTime.Now.ToString("dd-MM-yyyy HH:mm")}";

                entity.SmsMessageSaleOrderRels.Add(new SmsMessageSaleOrderRel()
                {
                    SaleOrderId = saleOrder.Id
                });

                await CreateAsync(entity);
            }

        }
    }
}
