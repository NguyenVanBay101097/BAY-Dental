using ApplicationCore.Entities;
using Dapper;
using Facebook.ApiClient.ApiEngine;
using Facebook.ApiClient.Constants;
using Facebook.ApiClient.Interfaces;
using Infrastructure.Data;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public void RunActivity(string db, Guid activityId)
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

                            foreach (var partnerPsid in partnerPsids)
                                SendFacebookMessage(page.PageAccesstoken, partnerPsid.PSId, content);
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private SendFacebookMessageReponse SendFacebookMessage(string access_token, string psid, string message)
        {
            var apiClient = new ApiClient(access_token, FacebookApiVersions.V6_0);
            var url = $"/me/messages";

            var request = (IPostRequest)ApiRequest.Create(ApiRequest.RequestType.Post, url, apiClient);
            request.AddParameter("message_type", "RESPONSE");
            request.AddParameter("recipient", JsonConvert.SerializeObject(new { id = psid }));
            request.AddParameter("message", JsonConvert.SerializeObject(new { text = message }));

            var response = request.Execute<SendFacebookMessageReponse>();
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

        public class SendFacebookMessageReponse
        {
            public string message_id { get; set; }
            public string recipient_id { get; set; }
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
