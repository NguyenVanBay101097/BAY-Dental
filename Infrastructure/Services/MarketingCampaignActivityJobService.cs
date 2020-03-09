using ApplicationCore.Entities;
using Dapper;
using Facebook.ApiClient.ApiEngine;
using Facebook.ApiClient.Constants;
using Facebook.ApiClient.Interfaces;
using Infrastructure.Data;
using Microsoft.Extensions.Options;
using MyERP.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZaloDotNetSDK;

namespace Infrastructure.Services
{
    public class MarketingCampaignActivityJobService: IMarketingCampaignActivityJobService
    {
        private readonly ConnectionStrings _connectionStrings;
        public MarketingCampaignActivityJobService(IOptions<ConnectionStrings> connectionStrings)
        {
            _connectionStrings = connectionStrings?.Value;
        }

        public async Task RunActivity(string db, Guid activityId)
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

                    var activity = conn.Query<MarketingCampaignActivity>("" +
                        "SELECT * " +
                        "FROM MarketingCampaignActivities " +
                        "where Id = @id" +
                        "", new { id = activityId }).FirstOrDefault();

                    var content = activity.Content;
                    if (string.IsNullOrEmpty(content))
                        return;

                    if (activity.ActivityType == "facebook")
                    {
                        var pages = GetFbPagesSendMessage(conn, activity);
                        foreach(var page in pages)
                        {
                            var partnerPsids = conn.Query<PartnerMapPSIDFacebookPage>("" +
                            "SELECT * " +
                                "FROM PartnerMapPSIDFacebookPages m " +
                                "where m.PageId = @pageId and m.PartnerId in (" +
                                    "SELECT p.Id " +
                                    "FROM Partners p " +
                                    "where p.Customer = 1 " +
                                ")", new { pageId = page.PageId }).ToList();


                            var tasks = partnerPsids.Select(x => SendFacebookMessage(page.PageAccesstoken, x.PSId, content));

                            var limit = 200;
                            var offset = 0;
                            var subTasks = tasks.Skip(offset).Take(limit);
                            while(subTasks.Any())
                            {
                                var result = await Task.WhenAll(subTasks);
                                offset += limit;
                                subTasks = tasks.Skip(offset).Take(limit);
                            }

                            //foreach (var partnerPsid in partnerPsids)
                            //{
                            //    var sendResult = SendFacebookMessage(page.PageAccesstoken, partnerPsid.PSId, content);
                            //    //if (sendResult == null)
                            //    //{
                            //    //    var insertRes = conn.Execute("insert into MarketingTraces(Id,ActivityId,Exception) values (@Id,@ActivityId,@Exception)", new { Id = GuidComb.GenerateComb(), ActivityId = activity.Id, Exception = DateTime.Now });
                            //    //}
                            //    //else
                            //    //{
                            //    //    var insertRes = conn.Execute("insert into MarketingTraces(ActivityId,Sent) values (@Id,@ActivityId,@Sent)", new { Id = GuidComb.GenerateComb(), ActivityId = activity.Id, Sent = DateTime.Now });
                            //    //}
                            //}
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private async Task<SendFacebookMessageReponse> SendFacebookMessage(string access_token, string psid, string message)
        {
            var apiClient = new ApiClient(access_token, FacebookApiVersions.V6_0);
            var url = $"/me/messages";

            var request = (IPostRequest)ApiRequest.Create(ApiRequest.RequestType.Post, url, apiClient);
            request.AddParameter("message_type", "RESPONSE");
            request.AddParameter("recipient", JsonConvert.SerializeObject(new { id = psid }));
            request.AddParameter("message", JsonConvert.SerializeObject(new { text = message }));

            var response = await request.ExecuteAsync<SendFacebookMessageReponse>();
            if (response.GetExceptions().Any())
            {
                var error = string.Join("; ", response.GetExceptions().Select(x => x.Message));
                return new SendFacebookMessageReponse() { error = error };
            }
            else
            {
                var result = response.GetResult();
                return result;
            }
        }

        public class SendFacebookMessageReponse
        {
            public string message_id { get; set; }
            public string recipient_id { get; set; }
            public string error { get; set; }
        }

        private IEnumerable<FacebookPage> GetFbPagesSendMessage(SqlConnection conn, MarketingCampaignActivity activity)
        {
            var pages = conn.Query<FacebookPage>("SELECT * FROM FacebookPages").ToList();
            return pages;
        }

        public class PartnerSendMessageResult
        {
            public string ZaloId { get; set; }
            public string Content { get; set; }
        }

        public class PartnerToDayAppointmentQuery
        {
            public DateTime Date { get; set; }
            public string PartnerName { get; set; }
            public string PartnerZaloId { get; set; }
        }

        public class MarketingCampaignActivityQuery
        {
            public string Condition { get; set; }

            public string Content { get; set; }

            public string ActivityType { get; set; }

            public DateTime? CampaignDateStart { get; set; }

            public bool? AutoTakeCoupon { get; set; }

            public Guid? CouponProgramId { get; set; }
        }
    }
}
