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
    public class SmsJobService : ISmsJobService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISmsSendMessageService _smsSendMessageService;
        public SmsJobService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, ISmsSendMessageService smsSendMessageService)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _smsSendMessageService = smsSendMessageService;
        }

        public async Task RunJob(string db, Guid configId)
        {
            await using var context = DbContextHelper.GetCatalogDbContext(db, _configuration);
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var smsConfig = await context.SmsConfigs.Where(x => x.Id == configId).AsQueryable().FirstOrDefaultAsync();

                if (smsConfig.TemplateId.HasValue)
                    smsConfig.Template = await context.SmsTemplates.Where(x => x.Id == smsConfig.TemplateId.Value).FirstOrDefaultAsync();

                if (smsConfig.IsAppointmentAutomation)
                    await RunAppointmentAutomatic(context, smsConfig);
                else if (smsConfig.IsBirthdayAutomation)
                    await RunBirthdayAutomatic(context, smsConfig);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        public async Task RunAppointmentAutomatic(CatalogDbContext context, SmsConfig config)
        {
            var now = DateTime.Now;
            var time = DateTime.Now;
            if (!string.IsNullOrEmpty(config.TypeTimeBeforSend))
            {
                var query = context.Appointments.AsQueryable();
                var listAppointments = await query.Where(x => !x.DateAppointmentReminder.HasValue
                  && x.DateTimeAppointment.HasValue
                  && x.State == "confirmed"
                  && x.DateTimeAppointment.Value >= now &&
                  ((config.TypeTimeBeforSend == "minute" && x.DateTimeAppointment.Value.AddMinutes(-config.TimeBeforSend) <= now) ||
                  (config.TypeTimeBeforSend == "hour" && x.DateTimeAppointment.Value.AddHours(-config.TimeBeforSend) <= now) ||
                  (config.TypeTimeBeforSend == "day" && x.DateTimeAppointment.Value.AddDays(-config.TimeBeforSend) <= now)))
                    .ToListAsync();
                if (listAppointments.Any())
                {
                    var partnerIds = listAppointments.Select(x => x.PartnerId);
                    var smsMessage = new SmsMessage();
                    smsMessage.SmsAccountId = config.SmsAccountId;
                    smsMessage.SmsCampaignId = config.SmsCampaignId;
                    smsMessage.Id = GuidComb.GenerateComb();
                    smsMessage.Name = $"Nhắc lịch hẹn khách hàng ngày {DateTime.Today.ToString("dd-MM-yyyy")}";
                    smsMessage.SmsTemplateId = config.TemplateId;
                    smsMessage.Body = config.Body;
                    smsMessage.State = "sending";
                    smsMessage.TypeSend = "automatic";
                    smsMessage.Date = config.DateSend;
                    context.SmsMessages.Add(smsMessage);
                    await context.SaveChangesAsync();
                    foreach (var id in partnerIds)
                    {
                        var rel = new SmsMessagePartnerRel()
                        {
                            PartnerId = id,
                            SmsMessageId = smsMessage.Id
                        };
                        smsMessage.Partners.Add(rel);
                    }
                    context.SmsMessagePartnerRels.AddRange(smsMessage.Partners);
                    await context.SaveChangesAsync();
                    smsMessage = await context.SmsMessages.Where(x => x.Id == smsMessage.Id).Include(x => x.SmsAccount).FirstOrDefaultAsync();
                    await _smsSendMessageService.CreateSmsMessageDetail(context, smsMessage, partnerIds, config.CompanyId);
                    foreach (var appointment in listAppointments)
                    {
                        appointment.DateAppointmentReminder = now;
                        context.Entry(appointment).State = EntityState.Modified;
                    }
                    await context.SaveChangesAsync();
                }
            }

        }

        public async Task RunBirthdayAutomatic(CatalogDbContext context, SmsConfig config)
        {
            var dayNow = DateTime.Today.AddDays(-config.TimeBeforSend).Day;
            var MonthNow = DateTime.Today.Month;
            var queryPartner = context.Partners.AsQueryable();
            var partners = await queryPartner.Where(x =>
            x.BirthMonth.HasValue && x.BirthMonth.Value == MonthNow &&
            x.BirthDay.HasValue && x.BirthDay.Value == dayNow
            ).ToListAsync();
            if (partners.Any())
            {
                var smsMessage = new SmsMessage();
                smsMessage.SmsAccountId = config.SmsAccountId;
                smsMessage.SmsCampaignId = config.SmsCampaignId;
                smsMessage.Id = GuidComb.GenerateComb();
                smsMessage.Name = $"Chúc mừng sinh nhật ngày {DateTime.Today.ToString("dd-MM-yyyy")}";
                smsMessage.SmsTemplateId = config.TemplateId;
                smsMessage.Body = config.Body;
                smsMessage.State = "waiting";
                smsMessage.TypeSend = "automatic";
                smsMessage.Date = config.DateSend;

                context.SmsMessages.Add(smsMessage);
                await context.SaveChangesAsync();
                foreach (var item in partners)
                {
                    var rel = new SmsMessagePartnerRel()
                    {
                        PartnerId = item.Id,
                        SmsMessageId = smsMessage.Id
                    };
                    smsMessage.Partners.Add(rel);
                }
                context.SmsMessagePartnerRels.AddRange(smsMessage.Partners);
                await context.SaveChangesAsync();
            }
        }

    }
}
