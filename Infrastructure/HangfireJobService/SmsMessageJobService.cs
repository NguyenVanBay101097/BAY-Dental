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
        public SmsMessageJobService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task RunJobFindSmsMessage(string db)
        {
            var now = DateTime.Now;
            var morning = DateTime.Today.AddHours(6).AddMinutes(0);
            var night = DateTime.Today.AddHours(22).AddMinutes(0);
            if (now < morning || now > night)
                return;

            await using var context = DbContextHelper.GetCatalogDbContext(db, _configuration);
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var smsMessageService = new SmsMessageService(context, null, null,null, null,
                            new EfRepository<SaleOrderLine>(context),
                            new EfRepository<SaleOrder>(context),
                            new EfRepository<SmsMessage>(context),
                            new EfRepository<Partner>(context),
                            new EfRepository<SmsMessageDetail>(context),
                            new EfRepository<Appointment>(context),
                            new EfRepository<SmsCampaign>(context));
             
                var query = context.SmsMessages.Where(
                    x =>
                    x.State == "in_queue" &&
                    x.ScheduleDate.HasValue &&
                    x.ScheduleDate.Value <= now
                ).Include(x => x.SmsAccount)
                .Include(x => x.SmsMessagePartnerRels)
                .Include(x => x.SmsMessageSaleOrderRels)
                .ThenInclude(x => x.SaleOrder);

                var smsMessages = await query.ToListAsync();
                var partnerIds = new List<Guid>();
                foreach (var item in smsMessages)
                {
                    await smsMessageService.ActionSend(item.Id);

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
