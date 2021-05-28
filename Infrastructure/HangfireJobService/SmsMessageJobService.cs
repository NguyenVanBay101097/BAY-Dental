using ApplicationCore.Entities;
using Infrastructure.Data;
using Infrastructure.Helpers;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyERP.Utilities;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;

namespace Infrastructure.HangfireJobService
{
    public class SmsMessageJobService : ISmsMessageJobService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SmsMessageJobService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task RunJobFindSmsMessage(string db, Guid companyId)
        {
            await using var context = DbContextHelper.GetCatalogDbContext(db, _configuration);
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var smsMessageDetailService = new SmsMessageDetailService(null, null, null, context, null);
                var now = DateTime.Now;
                var query = context.SmsMessages.Where(
                    x => x.TypeSend == "automatic" &&
                    x.State == "waiting" &&
                    x.Date.HasValue &&
                    x.Date.Value <= now
                ).Include(x => x.SmsAccount)
                .Include(x => x.SmsMessagePartnerRels)
                .Include(x => x.SmsMessageSaleOrderRels).ThenInclude(x => x.SaleOrder);
                var smsMessages = await query.ToListAsync();
                var partnerIds = new List<Guid>();
                foreach (var item in smsMessages)
                {
                    if (item.SmsMessagePartnerRels.Any() && item.ResModel == "partner")
                    {
                        partnerIds = item.SmsMessagePartnerRels.Select(x => x.PartnerId).ToList();
                    }
                    else if (item.SmsMessageSaleOrderRels.Any() && item.ResModel == "sale-order")
                    {
                        partnerIds = item.SmsMessageSaleOrderRels.Select(x => x.SaleOrder.PartnerId).ToList();
                    }

                    if (partnerIds.Any())
                        await smsMessageDetailService.CreateSmsMessageDetail(item, partnerIds, companyId);

                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }
    }
}
