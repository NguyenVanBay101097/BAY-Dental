using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Dapper;
using Facebook.ApiClient.ApiEngine;
using Facebook.ApiClient.Constants;
using Facebook.ApiClient.Interfaces;
using Hangfire;
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

        public async Task<FacebookScheduleAppointmentConfig> CreateFBSheduleConfig(FacebookScheduleAppointmentConfigSave val)
        {
            var userService = GetService<UserManager<ApplicationUser>>();
            var user = await userService.FindByIdAsync(UserId);

            var result = _mapper.Map<FacebookScheduleAppointmentConfig>(val);
            if (result.ScheduleType == "minutes" && result.ScheduleNumber <= 0)
            {
                throw new Exception("");
            }
            result.FBPageId = user.FacebookPageId.Value;
            await CreateAsync(result);

            return result;

        }

        [Obsolete]
        public async Task ActionStart(IEnumerable<Guid> ids)
        {


            var result = await SearchQuery(x => ids.Contains(x.Id) && x.AutoScheduleAppoint == false).ToListAsync();
            var tenant = _tenant != null ? _tenant.Hostname : "localhost";
            foreach (var item in result)
            {
                item.AutoScheduleAppoint = true;


                var ScheduleNumber = item.ScheduleNumber ?? 0;

                if (item.ScheduleType == "minutes")
                    //date = date.AddMinutes(-(ScheduleNumber));
                    RecurringJob.AddOrUpdate($"{tenant}-AutoMesFB", () => RunSendMessageFB(tenant, item.FBPageId, item.ScheduleType, item.ScheduleNumber.Value), Cron.MinuteInterval(ScheduleNumber));
                if (item.ScheduleType == "hours")
                    //date = date.AddHours(-(ScheduleNumber));
                    RecurringJob.AddOrUpdate($"{tenant}-AutoMesFB", () => RunSendMessageFB(tenant, item.FBPageId, item.ScheduleType, item.ScheduleNumber.Value), Cron.HourInterval(ScheduleNumber));

                //var jobId = BackgroundJob.Schedule(() => RunSendMessageFB(tenant, item.FBPageId), date);
                //item.JobId = jobId;
                //if (string.IsNullOrEmpty(item.JobId))
                //    throw new Exception("Can't not schedule job");

                await UpdateAsync(result);
            }
        }

        public async Task RunSendMessageFB(string db, Guid fbpageId, string scheduletype, int ScheduleNumber)
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

                    var fbshedule = conn.Query<FacebookScheduleAppointmentConfig>("" +
                        "Select * " +
                        "FROM FacebookScheduleAppointmentConfigs " +
                         "where FBPageId = @fbPageId" +
                         "", new { fbPageId = fbpageId }
                        ).FirstOrDefault();
                    if (page == null)
                        return;
                    var date = DateTime.Now;
                    if (fbshedule.ScheduleType == "minutes")
                    {
                        var datefrom = date.AddMinutes(-(ScheduleNumber + ScheduleNumber));
                        var dateto = date.AddMinutes(-(ScheduleNumber));
                        var fbSheduleConfigMinutes = conn.Query<PartnerAppointment>("" +
                            "Select app.PartnerId, fbuser.PSID , app.Date " +
                            "From Appointments as app  " +
                            "Inner Join FacebookUserProfiles as fbuser  On app.PartnerId = fbu.PartnerId " +
                            "Where fbu.FbPageId = @pageId AND app.Date BETWEEN @dateFrom AND @dateTo  ", new { pageId = fbpageId, dateFrom = datefrom, dateTo = dateto }).ToList();
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
                        var datefrom = date.AddHours(-(ScheduleNumber + 1));
                        var dateto = date.AddHours(-(ScheduleNumber));
                        var fbSheduleConfigHours = conn.Query<PartnerAppointment>("" +
                            "Select app.PartnerId, fbuser.PSID , app.Date " +
                            "From Appointments as app  " +
                            "Inner Join FacebookUserProfiles as fbuser  On app.PartnerId = fbu.PartnerId " +
                            "Where fbu.FbPageId = @pageId AND app.Date BETWEEN @dateFrom AND @dateTo  ", new { pageId = fbpageId, dateFrom = datefrom, dateTo = dateto }).ToList();
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
            request.AddParameter("messaging_type", "RESPONSE");
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

            var result = await SearchQuery(x => ids.Contains(x.Id) && x.AutoScheduleAppoint == true).ToListAsync();
            foreach (var fbshedule in result)
            {
                fbshedule.AutoScheduleAppoint = false;

                if (string.IsNullOrEmpty(fbshedule.JobId))
                    continue;
                BackgroundJob.Delete(fbshedule.JobId);

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
