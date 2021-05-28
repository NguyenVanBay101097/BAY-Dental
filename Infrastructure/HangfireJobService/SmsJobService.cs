using ApplicationCore.Entities;
using Hangfire;
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
        public SmsJobService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task RunJob(string db, Guid configId)
        {
            await using var context = DbContextHelper.GetCatalogDbContext(db, _configuration);
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {

                var smsConfig = await context.SmsConfigs.Where(x => x.Id == configId).FirstOrDefaultAsync();

                if (smsConfig.IsAppointmentAutomation)
                    BackgroundJob.Enqueue(() => RunAppointmentAutomatic(db, configId));

                else if (smsConfig.IsBirthdayAutomation)
                    BackgroundJob.Enqueue(() => RunBirthdayAutomatic(db, configId));

                //else if (smsConfig.IsCareAfterOrderAutomation)
                //    await RunCareAfterOrderAutomatic(context, smsConfig);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        public async Task RunAppointmentAutomatic(string db, Guid configId)
        {
            await using var context = DbContextHelper.GetCatalogDbContext(db, _configuration);
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var config = await context.SmsConfigs.Where(x => x.Id == configId).FirstOrDefaultAsync();
                if (config.TemplateId.HasValue)
                    config.Template = await context.SmsTemplates.Where(x => x.Id == config.TemplateId.Value).FirstOrDefaultAsync();
                var now = DateTime.Now;
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
                        var smsSendMessageService = new SmsMessageDetailService(null, null, null, context, null);
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
                        smsMessage.ResModel = "appointment";
                        smsMessage.Date = config.DateSend;
                        context.SmsMessages.Add(smsMessage);
                        await context.SaveChangesAsync();
                        foreach (var app in listAppointments)
                        {
                            var rel = new SmsMessageAppointmentRel()
                            {
                                AppointmentId = app.Id,
                                SmsMessageId = smsMessage.Id
                            };
                            smsMessage.SmsMessageAppointmentRels.Add(rel);
                        }
                        context.SmsMessageAppointmentRels.AddRange(smsMessage.SmsMessageAppointmentRels);
                        await context.SaveChangesAsync();
                        smsMessage = await context.SmsMessages.Where(x => x.Id == smsMessage.Id).Include(x => x.SmsAccount).FirstOrDefaultAsync();
                        await smsSendMessageService.CreateSmsMessageDetail(smsMessage, partnerIds, config.CompanyId);
                        foreach (var appointment in listAppointments)
                        {
                            appointment.DateAppointmentReminder = now;
                            context.Entry(appointment).State = EntityState.Modified;
                        }
                        await context.SaveChangesAsync();
                        transaction.Commit();

                    }
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task RunBirthdayAutomatic(string db, Guid configId)
        {
            await using var context = DbContextHelper.GetCatalogDbContext(db, _configuration);
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var config = await context.SmsConfigs.Where(x => x.Id == configId).FirstOrDefaultAsync();
                if (config.TemplateId.HasValue)
                    config.Template = await context.SmsTemplates.Where(x => x.Id == config.TemplateId.Value).FirstOrDefaultAsync();

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
                    smsMessage.ResModel = "birthday";
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
                        smsMessage.SmsMessagePartnerRels.Add(rel);
                    }
                    context.SmsMessagePartnerRels.AddRange(smsMessage.SmsMessagePartnerRels);
                    await context.SaveChangesAsync();
                }
                transaction.Commit();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task RunThanksCustomerAutomatic(CatalogDbContext context, SmsConfig config)
        {
            var now = DateTime.Now;
            if (!string.IsNullOrEmpty(config.TypeTimeBeforSend))
            {
                var orders = await context.SaleOrders.Where(
                    x => x.State == "done" &&
                    x.DateDone.HasValue &&
                    ((config.TypeTimeBeforSend == "hour" && x.DateDone.Value.AddHours(config.TimeBeforSend) <= now) ||
                    (config.TypeTimeBeforSend == "day" && x.DateDone.Value.AddDays(config.TimeBeforSend) <= now))).ToListAsync();

                if (orders.Any())
                {
                    var smsMessage = new SmsMessage();
                    smsMessage.SmsAccountId = config.SmsAccountId;
                    smsMessage.SmsCampaignId = config.SmsCampaignId;
                    smsMessage.Id = GuidComb.GenerateComb();
                    smsMessage.Name = $"Cảm ơn khách hàng tự động, ngày {DateTime.Today.ToString("dd-MM-yyyy")}";
                    smsMessage.SmsTemplateId = config.TemplateId;
                    smsMessage.Body = config.Body;
                    smsMessage.State = "waiting";
                    smsMessage.TypeSend = "automatic";
                    smsMessage.ResModel = "care-after-order";
                    smsMessage.Date = config.DateSend;
                    context.SmsMessages.Add(smsMessage);
                    await context.SaveChangesAsync();
                    foreach (var or in orders)
                    {
                        var rel = new SmsMessageSaleOrderRel()
                        {
                            SaleOrderId = or.Id,
                            SmsMessageId = smsMessage.Id
                        };
                        smsMessage.SmsMessageSaleOrderRels.Add(rel);
                    }
                    context.SmsMessageAppointmentRels.AddRange(smsMessage.SmsMessageAppointmentRels);
                    await context.SaveChangesAsync();
                    var partnerIds = orders.Select(x => x.PartnerId).ToList();
                    smsMessage = await context.SmsMessages.Where(x => x.Id == smsMessage.Id).Include(x => x.SmsAccount).FirstOrDefaultAsync();
                    //await _smsSendMessageService.CreateSmsMessageDetail(smsMessage, partnerIds, config.CompanyId);
                }
            }
        }

        public Task RunCareAfterOrderAutomatic(CatalogDbContext context, SmsConfig config)
        {
            throw new NotImplementedException();
        }
    }
}
