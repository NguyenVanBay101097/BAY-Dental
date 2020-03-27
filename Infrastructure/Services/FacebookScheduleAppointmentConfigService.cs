using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Dapper;
using Facebook.ApiClient.ApiEngine;
using Facebook.ApiClient.Constants;
using Facebook.ApiClient.Interfaces;
using Hangfire;
using Hangfire.Storage;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class FacebookScheduleAppointmentConfigService : BaseService<FacebookScheduleAppointmentConfig>, IFacebookScheduleAppointmentConfigService
    {
        private readonly IMapper _mapper;
        private readonly ConnectionStrings _connectionStrings;
        private readonly AppTenant _tenant;
        public FacebookScheduleAppointmentConfigService(IAsyncRepository<FacebookScheduleAppointmentConfig> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper, IOptions<ConnectionStrings> connectionStrings, ITenant<AppTenant> tenant)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _connectionStrings = connectionStrings?.Value;
            _tenant = tenant?.Value;
        }

       

        [Obsolete]
        public async Task<FacebookScheduleAppointmentConfig> CreateFBSheduleConfig(FacebookScheduleAppointmentConfigSave val)
        {
            var userService = GetService<UserManager<ApplicationUser>>();
            var user = await userService.FindByIdAsync(UserId);
            var lstId = new List<Guid>();
            var result = _mapper.Map<FacebookScheduleAppointmentConfig>(val);
            if (result.ScheduleType == "minutes" && result.ScheduleNumber <= 0 || result.ScheduleType == "hours" && result.ScheduleNumber <= 0)
            {
                throw new Exception("thời gian nhắc lịch không được nhỏ hơn hoặc bằng 0 !");
            }
            await CreateAsync(result);
            if (result.AutoScheduleAppoint == true)
            {
                lstId.Add(result.Id);
                await ActionStart(lstId);
            }
            return result;

        }

        [Obsolete]
        public async Task ActionStart(IEnumerable<Guid> ids)
        {

            var pageService = GetService<IFacebookPageService>();
            var result = await SearchQuery(x => ids.Contains(x.Id) && x.AutoScheduleAppoint == true).ToListAsync();
            var userService = GetService<UserManager<ApplicationUser>>();
            var user = await userService.FindByIdAsync(UserId);
            if (user.FacebookPageId == null)
            {
                throw new Exception($"Tài khoản {user.Name} chưa kết nối với Fanpage nào !");
            }
            var tenant = _tenant != null ? _tenant.Hostname : "localhost";
            foreach (var item in result)
            {
                item.AutoScheduleAppoint = true;

                var page = await pageService.SearchQuery(x => x.Id == user.FacebookPageId).FirstOrDefaultAsync();
                var ScheduleNumber = item.ScheduleNumber ?? 0;

                if (item.ScheduleType == "minutes")
                    //date = date.AddMinutes(-(ScheduleNumber));
                    RecurringJob.AddOrUpdate($"{tenant}-{page.PageName}-AutoMesFB", () => RunSendMessageFB(tenant, user.FacebookPageId.Value), Cron.MinuteInterval(ScheduleNumber));
                if (item.ScheduleType == "hours")
                    //date = date.AddHours(-(ScheduleNumber));
                    RecurringJob.AddOrUpdate($"{tenant}-{page.PageName}-AutoMesFB", () => RunSendMessageFB(tenant, user.FacebookPageId.Value), Cron.HourInterval(ScheduleNumber));

                item.RecurringJobId = $"{tenant}-{page.PageName}-AutoMesFB";
                if (string.IsNullOrEmpty(item.RecurringJobId))
                    throw new Exception("Can't not RecurringJob job");

                await UpdateAsync(result);
            }
        }

        [Obsolete]
        public async Task<FacebookScheduleAppointmentConfig> UpdateFBSheduleConfig(Guid id, FacebookScheduleAppointmentConfigSave val)
        {
            var lstId = new List<Guid>();
            var pageService = GetService<IFacebookPageService>();        
            var userService = GetService<UserManager<ApplicationUser>>();
            var user = await userService.FindByIdAsync(UserId);
            var page = await pageService.SearchQuery(x => x.Id == user.FacebookPageId).FirstOrDefaultAsync();
            if (user.FacebookPageId == null)
            {
                throw new Exception($"Tài khoản {user.Name} chưa kết nối với Fanpage nào !");
            }
            var tenant = _tenant != null ? _tenant.Hostname : "localhost";
            var fbshedule = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (fbshedule == null)
                throw new ArgumentNullException("fbshedule");
            if(fbshedule.RecurringJobId == null)
            {
                fbshedule.RecurringJobId = $"{tenant}-{page.PageName}-AutoMesFB";
            }
            if (fbshedule.ScheduleType != val.ScheduleType || fbshedule.ScheduleNumber.Value != val.ScheduleNumber || val.AutoScheduleAppoint == true)
            {
                if (val.ScheduleType == "minutes")
                    RecurringJob.AddOrUpdate(fbshedule.RecurringJobId, () => RunSendMessageFB(tenant, user.FacebookPageId.Value), Cron.MinuteInterval(val.ScheduleNumber.Value));
                if (val.ScheduleType == "hours")
                    RecurringJob.AddOrUpdate(fbshedule.RecurringJobId, () => RunSendMessageFB(tenant, user.FacebookPageId.Value), Cron.HourInterval(val.ScheduleNumber.Value));
            }

            var result = _mapper.Map(val,fbshedule);
            if (result.AutoScheduleAppoint == false)
            {
                lstId.Add(result.Id);
                await ActionStop(lstId);
            }

            await UpdateAsync(result);
            return result;

        }


        public async Task RunSendMessageFB(string db, Guid fbpageId)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.CatalogConnection);
            builder["Database"] = $"TMTDentalCatalogDb";

            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    conn.Open();
                    var page = conn.Query<FacebookPage>("" +
                        "SELECT * " +
                        "FROM FacebookPages " +
                        "where Id = @id" +
                        "", new { id = fbpageId }).FirstOrDefault();
                    if (page == null)
                        return;
                    var fbshedule = conn.Query<FacebookScheduleAppointmentConfig>("" +
                        "Select * " +
                        "FROM FacebookScheduleAppointmentConfigs " +
                         "where Id = @Id" +
                         "", new { Id = page.AutoConfigId }
                        ).FirstOrDefault();
                    if (fbshedule == null)
                        return;
                    var date = DateTime.Now;
                    if (fbshedule.ScheduleType == "minutes")
                    {
                        var datefrom = date;
                        var dateto = date.AddMinutes(fbshedule.ScheduleNumber.Value);
                        var fbSheduleConfigMinutes = conn.Query<PartnerAppointment>("" +
                            "Select app.PartnerId, fbuser.PSID , app.Date " +
                            "From Appointments as app " +
                            "Inner Join FacebookUserProfiles as fbuser  On app.PartnerId = fbuser.PartnerId " +
                            "Where fbuser.FbPageId = @pageId AND DATEADD(MINUTE,-@scheduleNumber,app.Date) BETWEEN @dateFrom AND @dateTo " + "", new { pageId = fbpageId, dateFrom = datefrom, dateTo = dateto, scheduleNumber = fbshedule.ScheduleNumber.Value }).ToList();
                        if (fbSheduleConfigMinutes == null)
                            return;
                        var tasks = fbSheduleConfigMinutes.Select(x => SendMessageFBAsync(page.PageAccesstoken, x.PSId, fbshedule.ContentMessage)).ToList();
                        var limit = 200;
                        var offset = 0;
                        var subTasks = tasks.Skip(offset).Take(limit).ToList();
                        while (subTasks.Any())
                        {
                            var result = await Task.WhenAll(subTasks);
                            offset += limit;
                            subTasks = tasks.Skip(offset).Take(limit).ToList();
                        }
                    }
                    else if (fbshedule.ScheduleType == "hours")
                    {
                        var datefrom = date;
                        var dateto = date.AddHours(fbshedule.ScheduleNumber.Value);
                        var fbSheduleConfigHours = conn.Query<PartnerAppointment>("" +
                            "Select app.PartnerId, fbuser.PSID , app.Date " +
                            "From Appointments as app " +
                            "Inner Join FacebookUserProfiles as fbuser  On app.PartnerId = fbuser.PartnerId " +
                            "Where fbuser.FbPageId = @pageId AND DATEADD(HOUR,-(@scheduleNumber),app.Date) BETWEEN @dateFrom AND @dateTo " + "", new { pageId = fbpageId, dateFrom = datefrom, dateTo = dateto, scheduleNumber = fbshedule.ScheduleNumber.Value }).ToList();
                        if (fbSheduleConfigHours == null)
                            return;
                        var tasks = fbSheduleConfigHours.Select(x => SendMessageFBAsync(page.PageAccesstoken, x.PSId, fbshedule.ContentMessage)).ToList();
                        var limit = 200;
                        var offset = 0;
                        var subTasks = tasks.Skip(offset).Take(limit).ToList();
                        while (subTasks.Any())
                        {
                            var result = await Task.WhenAll(subTasks);
                            offset += limit;
                            subTasks = tasks.Skip(offset).Take(limit).ToList();
                        }
                    }




                }
                catch
                {
                }
            }
        }

        public async Task<SendFacebookMessage> SendMessageFBAsync(string accessTokenPage, string psid, string message)
        {
            var errorMaessage = "";
            var apiClient = new ApiClient(accessTokenPage, FacebookApiVersions.V6_0);
            var url = $"me/messages";

            var request = (IPostRequest)ApiRequest.Create(ApiRequest.RequestType.Post, url, apiClient);
            request.AddParameter("messaging_type", "MESSAGE_TAG");
            request.AddParameter("tag", "CONFIRMED_EVENT_UPDATE");
            request.AddParameter("recipient", JsonConvert.SerializeObject(new { id = psid }));
            request.AddParameter("message", JsonConvert.SerializeObject(new { text = message }));
            

            var response = await request.ExecuteAsync<SendFacebookMessage>();

            if (response.GetExceptions().Any())
            {
                errorMaessage = string.Join("; ", response.GetExceptions().Select(x => x.Message));
                return new SendFacebookMessage() { error = errorMaessage };
            }
            else
            {
                var result = response.GetResult();

                return result;

            }



        }

        public async Task ActionStop(IEnumerable<Guid> ids)
        {
            List<RecurringJobDto> list;
            var result = await SearchQuery(x => ids.Contains(x.Id) && x.AutoScheduleAppoint == true).ToListAsync();
            foreach (var fbshedule in result)
            {
                fbshedule.AutoScheduleAppoint = false;

                if (string.IsNullOrEmpty(fbshedule.RecurringJobId))
                    continue;
                
                using (var connection = JobStorage.Current.GetConnection())
                {
                    list = connection.GetRecurringJobs();
                }
                var job = list?.FirstOrDefault(j => j.Id == fbshedule.RecurringJobId);  // jobId is the recurring job ID, whatever that is
                if (job != null && !string.IsNullOrEmpty(job.LastJobId))
                {
                    BackgroundJob.Delete(job.LastJobId);
                    RecurringJob.RemoveIfExists(job.LastJobId);
                }
                RecurringJob.RemoveIfExists(job.Id = fbshedule.RecurringJobId);
            }

            await UpdateAsync(result);
        }
    }
    public class PartnerAppointment
    {
        public Guid PartnerId { get; set; }
        public string PSId { get; set; }
        public DateTime Date { get; set; }
    }
    public class SendFacebookMessage
    {

        public string message_id { get; set; }
        public string recipient_id { get; set; }
        public string error { get; set; }
    }
}
