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
        private readonly ISmsSendMessageService _smsSendMessageService;
        public SmsMessageJobService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, ISmsSendMessageService smsSendMessageService)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _smsSendMessageService = smsSendMessageService;
        }
        public async Task RunJobFindSmsMessage(string db, Guid companyId)
        {
            await using var context = DbContextHelper.GetCatalogDbContext(db, _configuration);
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var now = DateTime.Now;
                var query = context.SmsMessages.Where(
                    x => x.TypeSend == "automatic" &&
                    x.State == "waiting" &&
                    x.Date.HasValue &&
                    x.Date.Value <= now
                ).Include(x => x.SmsAccount).Include(x => x.Partners);
                var smsMessages = await query.ToListAsync();
                foreach (var item in smsMessages)
                {
                    if (item.Partners.Any())
                    {
                        var partnerIds = item.Partners.Select(x => x.PartnerId).ToList();
                        await _smsSendMessageService.CreateSmsMessageDetail(context, item, partnerIds, companyId);
                    }
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
