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
                if (smsConfig.AppointmentTemplateId.HasValue)
                    smsConfig.AppointmentTemplate = await context.SmsTemplates.Where(x => x.Id == smsConfig.AppointmentTemplateId.Value).FirstOrDefaultAsync();

                if (smsConfig.BirthdayTemplateId.HasValue)
                    smsConfig.BirthdayTemplate = await context.SmsTemplates.Where(x => x.Id == smsConfig.BirthdayTemplateId.Value).FirstOrDefaultAsync();

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
            var query = context.Appointments.AsQueryable();
            var listAppointments = await query.Where(x => !x.DateAppointmentReminder.HasValue &&
              x.DateTimeAppointment.HasValue
              && x.DateTimeAppointment.Value >= now
              && x.DateTimeAppointment.Value.AddHours(-1) <= now).ToListAsync();
            if (listAppointments.Any())
            {
                var smsComposer = new SmsComposer();
                var partnerIds = listAppointments.Select(x => x.PartnerId);
                smsComposer.Id = GuidComb.GenerateComb();
                smsComposer.ResModel = "res.appointment";
                smsComposer.ResIds = string.Join(",", listAppointments.Select(x => x.Id));
                smsComposer.TemplateId = config.AppointmentTemplateId;
                smsComposer.Body = config.AppointmentTemplate.Body;
                context.SmsComposers.Add(smsComposer);
                await context.SaveChangesAsync();
                await _smsSendMessageService.CreateSmsSms(context, smsComposer, partnerIds, config.CompanyId);
                foreach (var appointment in listAppointments)
                {
                    appointment.DateAppointmentReminder = now;
                    context.Entry(appointment).State = EntityState.Modified;
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task RunBirthdayAutomatic(CatalogDbContext context, SmsConfig config)
        {
            var dayNow = DateTime.Today.Day;
            var MonthNow = DateTime.Today.Month;
            var query = context.Partners.AsQueryable();
            var partners = await query.Where(x =>
            x.BirthMonth.HasValue && x.BirthMonth.Value == MonthNow &&
            x.BirthDay.HasValue && x.BirthDay.Value == dayNow
            ).ToListAsync();
            if (partners.Any())
            {
                var smsComposer = new SmsComposer();
                smsComposer.Id = GuidComb.GenerateComb();
                smsComposer.ResModel = "res.partners";
                smsComposer.ResIds = string.Join(",", partners.Select(x => x.Id));
                smsComposer.TemplateId = config.BirthdayTemplateId;
                smsComposer.Body = config.BirthdayTemplate.Body;
                context.SmsComposers.Add(smsComposer);
                context.SaveChanges();
                await _smsSendMessageService.CreateSmsSms(context, smsComposer, partners.Select(x => x.Id), config.CompanyId);
            }
        }

    }
}
