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
            var user = userService.FindByIdAsync(UserId);
            
            var result = _mapper.Map<FacebookScheduleAppointmentConfig>(val);
            result.FBPageId = user.Result.FacebookPageId.Value;
            await CreateAsync(result);

            return result;

        }

        public async Task ActionStart(IEnumerable<Guid> ids)
        {
            var result = await SearchQuery(x => ids.Contains(x.Id) && x.AutoScheduleAppoint == false).ToListAsync();
            var tenant = _tenant != null ? _tenant.Hostname : "localhost";
            foreach (var item in result)
            {
                item.AutoScheduleAppoint = true;

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.CatalogConnection);
                builder["Database"] = $"TMTDentalCatalogDb";
                using (var conn = new SqlConnection(builder.ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        var fbSheduleConfigs = conn.Query<PartnerAppointment>("Select app.PartnerId, fbu.PSID ,app.Date From FacebookUserProfiles as fbu Inner Join Appointments as app On app.PartnerId = fbu.PartnerId Where fbu.FbPageId = @pageId", new { pageId = item.FBPageId }).ToList();
                        if (fbSheduleConfigs == null)
                            throw new Exception("Chưa có khách hàng nào kết nối Fanpage !");
                        foreach (var config in fbSheduleConfigs)
                        {
                            //thời gian được tính theo trước ngày tạo lịch hẹn 
                            var date = config.Date;
                            var ScheduleNumber = item.ScheduleNumber ?? 0;

                            if (item.ScheduleType == "minutes")
                                date = date.AddMinutes(-(ScheduleNumber));
                            if (item.ScheduleType == "hours")
                                date = date.AddHours(-(ScheduleNumber));
                            else if (item.ScheduleType == "days")
                                date = date.AddDays(-(ScheduleNumber));
                            else if (item.ScheduleType == "weeks")
                                date = date.AddDays((-(ScheduleNumber)) * 7);

                            var jobId = BackgroundJob.Schedule(() => RunSendMessageFB(tenant, item.FBPageId), date);
                            item.JobId = jobId;
                            if (string.IsNullOrEmpty(item.JobId))
                                throw new Exception("Can't not schedule job");
                        }
                    }
                    catch
                    {

                    }

                    
                }
                await UpdateAsync(result);
            }
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

                    var fbshedule = conn.Query<FacebookScheduleAppointmentConfig>("" +
                        "Select * " +
                        "FROM FacebookScheduleAppointmentConfigs " +
                         "where FBPageId = @fbPageId" +
                         "", new { fbPageId = fbpageId }
                        ).FirstOrDefault();
                    if (page == null)
                        return;
                    var fbSheduleConfig = conn.Query<PartnerAppointment>("Select app.PartnerId, fbu.PSID , app.DateCreated  From FacebookUserProfiles as fbu Join Appointments as app On app.PartnerId = fbu.PartnerId Where fbu.FbPageId = @pageId", new { pageId = fbpageId }).ToList();
                    
                    var tasks = fbSheduleConfig.Select(x => SendMessageFBAsync(page.PageAccesstoken, x.PSId, fbshedule.ContentMessage )).ToList();                  
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
                catch
                {
                }
            }
        }

        public async Task<SendFacebookMessage> SendMessageFBAsync(string accessTokenPage, string psid, string message )
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
