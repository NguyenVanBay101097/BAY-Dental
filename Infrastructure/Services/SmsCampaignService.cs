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
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));

            return query;
        }

        public async Task<PagedResult2<SmsCampaignBasic>> GetPaged(SmsCampaignPaged val)
        {
            var query = GetQueryable(val);
            var totalItems = await query.CountAsync();
            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();
            return new PagedResult2<SmsCampaignBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<SmsCampaignBasic>>(items)
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
    }
}
