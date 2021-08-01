using ApplicationCore.Entities;
using Hangfire;
using Hangfire.Storage;
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
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace Infrastructure.HangfireJobService
{
    public class SmsJobService : ISmsJobService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        public SmsJobService(IConfiguration configuration,
            IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
        }

        public async Task RunAppointmentAutomatic(string db, Guid companyId)
        {
            await using var context = DbContextHelper.GetCatalogDbContext(db, _configuration);
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var config = await context.SmsAppointmentAutomationConfigs.Where(x => x.CompanyId.HasValue && x.CompanyId.Value == companyId && x.Active == true)
                    .Include(x => x.Template)
                    .FirstOrDefaultAsync();
                if (config == null)
                    return;

                DateTime? lastExecution = config.LastCron;
                var now = DateTime.Now;
                config.LastCron = now;
                context.Entry(config).State = EntityState.Modified;
                await context.SaveChangesAsync();

                var listAppointments = await context.Appointments.Where(x => x.CompanyId == config.CompanyId
                      && x.DateTimeAppointment.HasValue
                      && x.State == "confirmed"
                      && x.DateTimeAppointment.Value >= now &&
                      ((config.TypeTimeBeforSend == "hour" && x.DateTimeAppointment.Value.AddHours(-config.TimeBeforSend) < now && (!lastExecution.HasValue || x.DateTimeAppointment.Value.AddHours(-config.TimeBeforSend) > lastExecution)) ||
                      (config.TypeTimeBeforSend == "day" && x.DateTimeAppointment.Value.AddDays(-config.TimeBeforSend) < now && (!lastExecution.HasValue || x.DateTimeAppointment.Value.AddDays(-config.TimeBeforSend) > lastExecution))))
                        .ToListAsync();
                if (listAppointments.Any())
                {
                    var smsMessageService = new SmsMessageService(context, null, null, _clientFactory, null,
                        new EfRepository<SaleOrderLine>(context),
                        new EfRepository<SaleOrder>(context),
                        new EfRepository<SmsMessage>(context),
                        new EfRepository<Partner>(context),
                        new EfRepository<SmsMessageDetail>(context),
                        new EfRepository<Appointment>(context),
                        new EfRepository<SmsCampaign>(context));

                    var smsMessage = new SmsMessage();
                    smsMessage.SmsAccountId = config.SmsAccountId;
                    smsMessage.SmsCampaignId = config.SmsCampaignId;
                    smsMessage.Name = $"Nhắc lịch hẹn";
                    smsMessage.SmsTemplateId = config.TemplateId;
                    smsMessage.Body = config.Template.Body;
                    smsMessage.ResModel = "appointment";
                    smsMessage.CompanyId = config.CompanyId;

                    foreach (var app in listAppointments)
                    {
                        var rel = new SmsMessageAppointmentRel()
                        {
                            AppointmentId = app.Id,
                        };
                        smsMessage.SmsMessageAppointmentRels.Add(rel);
                    }

                    context.SmsMessages.Add(smsMessage);
                    await context.SaveChangesAsync();

                    await smsMessageService.ActionSend(smsMessage.Id);
                }

                transaction.Commit();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task RunBirthdayAutomatic(string db, Guid companyId)
        {
            await using var context = DbContextHelper.GetCatalogDbContext(db, _configuration);
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var config = await context.SmsBirthdayAutomationConfigs.Where(x => x.CompanyId.HasValue && x.CompanyId.Value == companyId && x.Active == true)
                    .Include(x => x.Template)
                    .FirstOrDefaultAsync();

                if (config == null)
                    return;

                var birthdate = DateTime.Today.AddDays(config.DayBeforeSend);
                var partners = await context.Partners.Where(x =>
                x.CompanyId == config.CompanyId &&
                x.BirthMonth.HasValue && x.BirthMonth.Value == birthdate.Month &&
                x.BirthDay.HasValue && x.BirthDay.Value == birthdate.Day
                ).ToListAsync();
                if (partners.Any())
                {
                    var smsMessageService = new SmsMessageService(context, null, null, _clientFactory, null,
                            new EfRepository<SaleOrderLine>(context),
                            new EfRepository<SaleOrder>(context),
                            new EfRepository<SmsMessage>(context),
                            new EfRepository<Partner>(context),
                            new EfRepository<SmsMessageDetail>(context),
                            new EfRepository<Appointment>(context),
                            new EfRepository<SmsCampaign>(context));
                    var smsMessage = new SmsMessage();
                    smsMessage.SmsAccountId = config.SmsAccountId;
                    smsMessage.SmsCampaignId = config.SmsCampaignId;
                    smsMessage.CompanyId = config.CompanyId;
                    smsMessage.Name = $"Chúc mừng sinh nhật";
                    smsMessage.SmsTemplateId = config.TemplateId;
                    smsMessage.Body = config.Template.Body;
                    smsMessage.ResModel = "partner";

                    foreach (var item in partners)
                    {
                        var rel = new SmsMessagePartnerRel()
                        {
                            PartnerId = item.Id,
                        };
                        smsMessage.SmsMessagePartnerRels.Add(rel);
                    }

                    context.SmsMessages.Add(smsMessage);
                    await context.SaveChangesAsync();
                  
                    await smsMessageService.ActionSend(smsMessage.Id);
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
                var config = await context.SmsCareAfterOrderAutomationConfigs
                    .Where(x => x.Id == configId && x.Active == true)
                    .Include(x => x.Template)
                    .Include(x => x.SmsConfigProductCategoryRels)
                    .Include(x => x.SmsConfigProductRels)
                    .FirstOrDefaultAsync();

                if (config == null)
                    return;

                DateTime? lastExecution = config.LastCron;
                var now = DateTime.Now;
                config.LastCron = now;
                context.Entry(config).State = EntityState.Modified;
                await context.SaveChangesAsync();

                var today = config.TypeTimeBeforSend == "day" ? DateTime.Today.AddDays(-config.TimeBeforSend) : DateTime.Today.AddMonths(-config.TimeBeforSend);
                var productIds = config.SmsConfigProductRels.Select(x => x.ProductId).ToList();
                var categIds = config.SmsConfigProductCategoryRels.Select(x => x.ProductCategoryId).ToList();

                var lines = await context.SaleOrderLines.Where(
                    x => x.Order.State == "done" &&
                    x.CompanyId == config.CompanyId &&
                    ((config.ApplyOn == "product" && productIds.Contains(x.ProductId.Value)) ||
                    (config.ApplyOn == "product_category" && categIds.Contains(x.Product.CategId.Value))) &&
                    ((config.TypeTimeBeforSend == "month" && x.Order.DateDone.Value.AddMonths(config.TimeBeforSend) < now && (!lastExecution.HasValue || x.Order.DateDone.Value.AddMonths(config.TimeBeforSend) > lastExecution)) ||
                    (config.TypeTimeBeforSend == "day" && x.Order.DateDone.Value.AddDays(config.TimeBeforSend) < now && (!lastExecution.HasValue || x.Order.DateDone.Value.AddDays(config.TimeBeforSend) > lastExecution)))
                 ).ToListAsync();

                if (lines.Any())
                {
                    var smsMessageService = new SmsMessageService(context, null, null, _clientFactory, null,
                         new EfRepository<SaleOrderLine>(context),
                         new EfRepository<SaleOrder>(context),
                         new EfRepository<SmsMessage>(context),
                         new EfRepository<Partner>(context),
                         new EfRepository<SmsMessageDetail>(context),
                         new EfRepository<Appointment>(context),
                         new EfRepository<SmsCampaign>(context));
                    var smsMessage = new SmsMessage();
                    smsMessage.SmsAccountId = config.SmsAccountId;
                    smsMessage.SmsCampaignId = config.SmsCampaignId;
                    smsMessage.CompanyId = config.CompanyId;
                    smsMessage.Name = $"Chăm sóc sau điều trị";
                    smsMessage.SmsTemplateId = config.TemplateId;
                    smsMessage.Body = config.Template.Body;
                    smsMessage.ResModel = "sale-order-line";

                    foreach (var line in lines)
                    {
                        var rel = new SmsMessageSaleOrderLineRel()
                        {
                            SaleOrderLineId = line.Id,
                        };
                        smsMessage.SmsMessageSaleOrderLineRels.Add(rel);
                    }

                    context.SmsMessages.Add(smsMessage);
                    await context.SaveChangesAsync();
                   
                    await smsMessageService.ActionSend(smsMessage.Id);
                }
                transaction.Commit();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }

        }

        public async Task RunThankCustomerAutomatic(string db, Guid companyId)
        {
            await using var context = DbContextHelper.GetCatalogDbContext(db, _configuration);
            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var config = await context.SmsThanksCustomerAutomationConfigs.Where(x => x.CompanyId.HasValue && x.CompanyId.Value == companyId && x.Active == true)
                    .Include(x => x.Template)
                    .FirstOrDefaultAsync();

                if (config == null)
                    return;

                var now = DateTime.Now;
                DateTime? lastExecution = config.LastCron;
                config.LastCron = now;
                context.Entry(config).State = EntityState.Modified;
                await context.SaveChangesAsync();

                var orders = await context.SaleOrders.Where(x => x.State == "done" &&
                x.CompanyId == config.CompanyId &&
                 ((config.TypeTimeBeforSend == "month" && x.DateDone.Value.AddMonths(config.TimeBeforSend) < now && (!lastExecution.HasValue || x.DateDone.Value.AddMonths(config.TimeBeforSend) > lastExecution)) ||
                    (config.TypeTimeBeforSend == "day" && x.DateDone.Value.AddDays(config.TimeBeforSend) < now && (!lastExecution.HasValue || x.DateDone.Value.AddDays(config.TimeBeforSend) > lastExecution)))
                ).ToListAsync();
                if (orders.Any())
                {
                    var smsMessageService = new SmsMessageService(context, null, null, _clientFactory, null,
                            new EfRepository<SaleOrderLine>(context),
                            new EfRepository<SaleOrder>(context),
                            new EfRepository<SmsMessage>(context),
                            new EfRepository<Partner>(context),
                            new EfRepository<SmsMessageDetail>(context),
                            new EfRepository<Appointment>(context),
                            new EfRepository<SmsCampaign>(context));
                    var smsMessage = new SmsMessage();
                    smsMessage.SmsAccountId = config.SmsAccountId;
                    smsMessage.SmsCampaignId = config.SmsCampaignId;
                    smsMessage.CompanyId = config.CompanyId;
                    smsMessage.Name = $"Tin nhắn cảm ơn";
                    smsMessage.SmsTemplateId = config.TemplateId;
                    smsMessage.Body = config.Template.Body;
                    smsMessage.ResModel = "sale-order";

                    foreach (var item in orders)
                    {
                        var rel = new SmsMessageSaleOrderRel()
                        {
                            SaleOrderId = item.Id,
                        };
                        smsMessage.SmsMessageSaleOrderRels.Add(rel);
                    }

                    context.SmsMessages.Add(smsMessage);
                    await context.SaveChangesAsync();

                    await smsMessageService.ActionSend(smsMessage.Id);
                }
                transaction.Commit();
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
