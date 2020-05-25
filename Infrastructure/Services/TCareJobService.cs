using ApplicationCore.Entities;
using Dapper;
using Facebook.ApiClient.ApiEngine;
using Facebook.ApiClient.Constants;
using Facebook.ApiClient.Interfaces;
using Hangfire;
using Infrastructure.Data;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZaloDotNetSDK;

namespace Infrastructure.Services
{
    public class TCareJobService : ITCareJobService
    {
        private readonly ConnectionStrings _connectionStrings;
        private readonly IFacebookMessageSender _fbMessageSender;
        public TCareJobService(IOptions<ConnectionStrings> connectionStrings, IFacebookMessageSender fbMessageSender)
        {

            _connectionStrings = connectionStrings?.Value;
            _fbMessageSender = fbMessageSender;
        }

        public void Run(string db)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.CatalogConnection);
            builder["Database"] = $"TMTDentalCatalogDb__{db}";
            if (db == "localhost")
                builder["Database"] = "TMTDentalCatalogDb";
            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    conn.Open();
                    var date = DateTime.UtcNow;
                    var campaigns = conn.Query("SELECT * FROM TCareCampaigns").ToList();
                    foreach (var campaign in campaigns)
                    {
                        var rules = conn.Query<TCareRule>("SELECT * FROM TCareRules WHERE CampaignId = @id", new { id = campaign.Id }).ToList();
                        var partner_ids = SearchPartnerRules(rules, conn);
                        var messaging = conn.Query<TCareMessaging>("SELECT * FROM TCareMessagings WHERE TCareCampaignId = @id", new { id = campaign.Id }).FirstOrDefault();
                        if (messaging.MethodType == "interval")
                        {
                            var intervalNumber = messaging.IntervalNumber ?? 0;
                            if (messaging.IntervalType == "hours")
                                date = date.AddHours(-intervalNumber);
                            else if (messaging.IntervalType == "minutes")
                                date = date.AddMinutes(-intervalNumber);
                            else if (messaging.IntervalType == "days")
                                date = date.AddDays(-intervalNumber);
                            else if (messaging.IntervalType == "months")
                                date = date.AddMonths(-intervalNumber);
                            else if (messaging.IntervalType == "weeks")
                                date = date.AddDays(-intervalNumber * 7);
                            var jobId = BackgroundJob.Schedule(() => SendMessageSocial(messaging.Id, messaging.Content, partner_ids, conn), date);
                        }
                        else
                        {
                            if (!messaging.SheduleDate.HasValue)
                                throw new Exception("Không tìm thấy thời gian cố định . Vui lòng kiểm tra lại !!!");
                            date = messaging.SheduleDate.Value;
                            var jobId = BackgroundJob.Schedule(() => SendMessageSocial(messaging.Id, messaging.Content, partner_ids, conn), date);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public IEnumerable<Guid> SearchPartnerRules(IEnumerable<TCareRule> rules, SqlConnection conn)
        {
            IEnumerable<Guid> res = null;
            foreach (var rule in rules)
            {
                var tmp = SearchPartnerIds(rule, conn);
                if (res == null)
                {
                    res = tmp;
                    continue;
                }

                res = res.Intersect(tmp);
            }

            return res;
        }

        public IEnumerable<Guid> SearchPartnerIds(TCareRule rule, SqlConnection conn)
        {
            if (rule.Type == "birthday")
            {
                var today = DateTime.Today;
                var properties = conn.Query<TCareProperty>("SELECT * FROM TCareProperties WHERE RuleId = @id", new { id = rule.Id }).ToList();
                var beforeDays = 0;
                var prop = properties.FirstOrDefault(x => x.Name == "BeforeDays");
                if (prop != null)
                    beforeDays = prop.ValueInteger ?? 0;
                var date = today.AddDays(-beforeDays);

                var partner_ids = conn.Query<Guid>("SELECT Id FROM Partners WHERE Customer = 1 AND BirthDay = @day AND BirthMonth = @month", new { day = date.Day, month = date.Month }).ToList();
                return partner_ids;
            }

            return new List<Guid>();
        }

        private async Task SendMessageSocial(Guid? messagingId = null, string content = null, IEnumerable<Guid> partIds = null,
            SqlConnection conn = null)
        {
            var lstpartnerId = new List<Guid>();
            var profiles = new List<FacebookUserProfile>().AsEnumerable();
            var messaging = conn.Query<TCareMessaging>("SELECT * FROM TCareMessagings WHERE TCareCampaignId = @id", new { id = messagingId }).FirstOrDefault();
            if (messaging == null)
                return;

            var channelSocial = conn.Query<FacebookPage>("" +
                 "SELECT * " +
                 "FROM FacebookPages " +
                 "where Id = @id" +
                 "", new { id = messaging.ChannelSocialId }).FirstOrDefault();

            if (channelSocial == null)
                return;

            profiles = GetUserProfiles(messaging.ChannelSocialId.Value, partIds, conn);
            if (profiles == null)
                return;
            if (channelSocial.Type == "facebook")
            {
                var tasks = profiles.Select(x => SendMessageFacebookTextAsync(content, x.PSID, channelSocial.PageAccesstoken)).ToList();
                var limit = 200;
                var offset = 0;
                var subTasks = tasks.Skip(offset).Take(limit).ToList();
                while (subTasks.Any())
                {
                    await Task.WhenAll(subTasks);
                    offset += limit;
                    subTasks = tasks.Skip(offset).Take(limit).ToList();
                }
            }
            else if (channelSocial.Type == "zalo")
            {
                var zaloClient = new ZaloClient(channelSocial.PageAccesstoken);
                var tasks = profiles.Select(x => zaloClient.sendTextMessageToUserId(x.PSID, content)).ToList();
            }

            //lấy ra partnerids của danh sách profiles mới gửi
            partIds = partIds.Where(x=>!profiles.Any(s=>s.PartnerId == x)).ToList();

            // check điều kiện kênh ưu tiên
            if (messaging.ChannelType == "priority")
            {
                var channelSocials = conn.Query<FacebookPage>("" +
                     "SELECT * " +
                     "FROM FacebookPages " +
                     "where Id != @id" +
                     "", new { id = messaging.ChannelSocialId }).ToList();
               
                foreach (var channel in channelSocials)
                {               
                    if (partIds.Count() == 0)
                        break;
                    profiles = GetUserProfiles(channel.Id, partIds, conn);
                    if (profiles == null)
                        return;
                    if (channel.Type == "facebook")
                    {
                        var tasks = profiles.Select(x => SendMessageFacebookTextAsync(content, x.PSID, channel.PageAccesstoken)).ToList();
                        var limit = 200;
                        var offset = 0;
                        var subTasks = tasks.Skip(offset).Take(limit).ToList();
                        while (subTasks.Any())
                        {
                            await Task.WhenAll(subTasks);
                            offset += limit;
                            subTasks = tasks.Skip(offset).Take(limit).ToList();
                        }
                    }
                    else if (channel.Type == "zalo")
                    {
                        var zaloClient = new ZaloClient(channelSocial.PageAccesstoken);
                        var tasks = profiles.Select(x => zaloClient.sendTextMessageToUserId(x.PSID, content)).ToList();
                    }
                    partIds = partIds.Where(x => !profiles.Any(s => s.PartnerId == x)).ToList();
                }



            }






        }


        private static IEnumerable<FacebookUserProfile> GetUserProfiles(Guid channelSocialId, IEnumerable<Guid> partIds,
           SqlConnection conn = null)
        {
            var builder = new SqlBuilder();
            var sqltemplate = builder.AddTemplate("SELECT /**select**/ FROM FacebookUserProfiles us /**leftjoin**/ /**where**/ /**orderby**/ ");

            builder.Select("us.id , us.PSID , us.Name , us.PartnerId ");
            builder.Where("us.FbPageId = @PageId ", new { PageId = channelSocialId });
            var iUserprofiles = conn.Query<FacebookUserProfile>(sqltemplate.RawSql, sqltemplate.Parameters).ToList();
            // lấy danh sách userprofiles theo filter
            //var profiles = GetProfilesActivity(activity, pageId, conn);
            var lstUserProfile = iUserprofiles.Where(x => partIds.Contains(x.PartnerId.Value)).ToList();

            return lstUserProfile;

        }



        private async Task<SendFacebookMessageReponse> SendMessageFacebookTextAsync(string message, string psid, string access_token, string tag = "ACCOUNT_UPDATE")
        {
            var apiClient = new ApiClient(access_token, FacebookApiVersions.V6_0);
            var url = $"/me/messages";

            var request = (IPostRequest)ApiRequest.Create(ApiRequest.RequestType.Post, url, apiClient);
            request.AddParameter("message_type", "MESSAGE_TAG");
            request.AddParameter("recipient", JsonConvert.SerializeObject(new { id = psid }));
            request.AddParameter("message", JsonConvert.SerializeObject(new { text = message }));
            request.AddParameter("tag", tag);

            try
            {
                var response = await request.ExecuteAsync<SendFacebookMessageReponse>();
                if (response.GetExceptions().Any())
                {
                    return null;
                }
                else
                {
                    var result = response.GetResult();
                    return result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }



    public class PartnerSendMessageResult
    {
        public string ZaloId { get; set; }
        public string Content { get; set; }
    }
}
