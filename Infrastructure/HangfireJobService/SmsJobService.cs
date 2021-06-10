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
                        var smsMessageService = new SmsMessageService(context, null, null,
                            new EfRepository<SaleOrderLine>(context),
                            new EfRepository<SaleOrder>(context),
                            new EfRepository<SmsMessage>(context),
                            new EfRepository<Partner>(context),
                            new EfRepository<SmsMessageDetail>(context),
                            new EfRepository<Appointment>(context));

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

                        await smsMessageService.ActionSend(smsMessage.Id);

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
                    var smsMessageService = new SmsMessageService(context, null, null,
                            new EfRepository<SaleOrderLine>(context),
                            new EfRepository<SaleOrder>(context),
                            new EfRepository<SmsMessage>(context),
                            new EfRepository<Partner>(context),
                            new EfRepository<SmsMessageDetail>(context),
                            new EfRepository<Appointment>(context));
                    var smsMessage = new SmsMessage();
                    smsMessage.SmsAccountId = config.SmsAccountId;
                    smsMessage.SmsCampaignId = config.SmsCampaignId;
                    smsMessage.CompanyId = config.CompanyId;
                    smsMessage.Id = GuidComb.GenerateComb();
                    smsMessage.Name = $"Chúc mừng sinh nhật ngày {DateTime.Today.ToString("dd-MM-yyyy")}";
                    smsMessage.SmsTemplateId = config.TemplateId;
                    smsMessage.Body = config.Body;
                    smsMessage.State = "waiting";
                    smsMessage.TypeSend = "automatic";
                    smsMessage.ResModel = "partner";
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

                    await smsMessageService.ActionSend(smsMessage.Id);

                    await context.SaveChangesAsync();
                    transaction.Commit();
                }
                transaction.Commit();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task RunCareAfterOrderAutomatic(string db, Guid configId)
        {
            await using var context = DbContextHelper.GetCatalogDbContext(db, _configuration);
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var config = await context.SmsConfigs
                    .Where(x => x.Id == configId && x.IsCareAfterOrderAutomation == true)
                    .Include(x => x.SmsConfigProductCategoryRels)
                    .Include(x => x.SmsConfigProductRels)
                    .FirstOrDefaultAsync();
                if (config != null)
                {

                    var today = config.TypeTimeBeforSend == "day" ? DateTime.Today.AddDays(-config.TimeBeforSend) : DateTime.Today.AddMonths(-config.TimeBeforSend);
                    var productIds = new List<Guid>();
                    if (config.SmsConfigProductCategoryRels.Any())
                    {
                        var categoryIds = config.SmsConfigProductCategoryRels.Select(x => x.ProductCategoryId).ToList();
                        productIds = context.Products.Where(x => x.CategId.HasValue && categoryIds.Contains(x.CategId.Value)).Select(x => x.Id).ToList();
                    }
                    else
                    {
                        productIds = config.SmsConfigProductRels.Select(x => x.ProductId).ToList();
                    }

                    var lines = await context.SaleOrderLines.Where(
                        x => x.Order.State == "done" &&
                        x.OrderPartnerId.HasValue &&
                        x.ProductId.HasValue &&
                        productIds.Contains(x.ProductId.Value) &&
                        x.Order.DateDone.HasValue &&
                        x.Order.DateDone.Value.Date == today
                     ).ToListAsync();
                    if (lines.Any())
                    {
                        var smsMessageService = new SmsMessageService(context, null, null,
                             new EfRepository<SaleOrderLine>(context),
                             new EfRepository<SaleOrder>(context),
                             new EfRepository<SmsMessage>(context),
                             new EfRepository<Partner>(context),
                             new EfRepository<SmsMessageDetail>(context),
                             new EfRepository<Appointment>(context));
                        var smsMessage = new SmsMessage();
                        smsMessage.SmsAccountId = config.SmsAccountId;
                        smsMessage.SmsCampaignId = config.SmsCampaignId;
                        smsMessage.CompanyId = config.CompanyId;
                        smsMessage.Id = GuidComb.GenerateComb();
                        smsMessage.Name = $"Gửi tin nhắn  chăm sóc sau điều trị dịch vụ ngày {DateTime.Today.ToString("dd-MM-yyyy")}";
                        smsMessage.SmsTemplateId = config.TemplateId;
                        smsMessage.Body = config.Body;
                        smsMessage.State = "sending";
                        smsMessage.TypeSend = "automatic";
                        smsMessage.ResModel = "sale-order-line";
                        smsMessage.Date = config.DateSend;
                        context.SmsMessages.Add(smsMessage);
                        await context.SaveChangesAsync();
                        foreach (var line in lines)
                        {
                            var rel = new SmsMessageSaleOrderLineRel()
                            {
                                SaleOrderLineId = line.Id,
                                SmsMessageId = smsMessage.Id
                            };
                            smsMessage.SmsMessageSaleOrderLineRels.Add(rel);
                        }
                        context.SmsMessageSaleOrderLineRels.AddRange(smsMessage.SmsMessageSaleOrderLineRels);
                        await context.SaveChangesAsync();
                      
                        await smsMessageService.ActionSend(smsMessage.Id);
                    }
                    transaction.Commit();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }

        }
    }
}
