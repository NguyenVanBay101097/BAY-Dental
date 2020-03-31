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

                    if (activity == null)
                        return;

                    var campaign = conn.Query<MarketingCampaign>("" +
                        "SELECT * " +
                        "FROM MarketingCampaigns " +
                        "where Id = @id" +
                        "", new { id = activity.CampaignId }).FirstOrDefault();

                    if (campaign == null || !campaign.FacebookPageId.HasValue)
                        return;

                    if (!activity.MessageId.HasValue)
                        return;

                    var message_obj = GetMessageForSendApi(conn, activity.MessageId.Value);

                    if (activity.ActivityType == "message")
                    {
                        var page = conn.Query<FacebookPage>("" +
                        "SELECT * " +
                        "FROM FacebookPages " +
                        "where Id = @id" +
                        "", new { id = campaign.FacebookPageId }).FirstOrDefault();

                        if (page == null)
                            return;

                        var profiles = conn.Query<FacebookUserProfile>("" +
                            "SELECT * " +
                                "FROM FacebookUserProfiles m " +
                                "where m.FbPageId = @pageId and m.PartnerId in (" +
                                    "SELECT p.Id " +
                                    "FROM Partners p " +
                                    "where p.Customer = 1 " +
                                ")", new { pageId = page.Id }).ToList();


                        var tasks = profiles.Select(x => SendFacebookMessage(page.PageAccesstoken, x.PSID, message_obj, activity.Id, conn)).ToList();

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
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public object GetMessageForSendApi(SqlConnection conn, Guid messageId)
        {
            var message = conn.Query<MarketingMessage>("" +
                      "SELECT * " +
                      "FROM MarketingMessages " +
                      "where Id = @id" +
                      "", new { id = messageId }).FirstOrDefault();

            if (message.Type == "facebook")
            {
                if (message.Template == "button")
                {
                    var buttons = conn.Query<MarketingMessageButton>("" +
                      "SELECT * " +
                      "FROM MarketingMessageButtons " +
                      "where MessageId = @id" +
                      "", new { id = messageId }).ToList();

                    var buttonList = new List<object>();
                    foreach(var button in buttons)
                    {
                        if (button.Type == "web_url")
                        {
                            buttonList.Add(new
                            {
                                type = button.Type,
                                url = button.Url,
                                title = button.Title
                            });
                        }
                        else if (button.Type == "phone_number")
                        {
                            buttonList.Add(new
                            {
                                type = button.Type,
                                payload = button.Payload,
                                title = button.Title
                            });
                        }
                    }

                    var res = new
                    {
                        attachment = new
                        {
                            type = "template",
                            payload = new
                            {
                                template_type = "button",
                                text = message.Text,
                                buttons = buttonList
                            }
                        }
                    };

                    return res;
                }
                else if (message.Template == "text")
                {
                    var res = new
                    {
                        text = message.Text
                    };

                    return res;
                }
            }

            throw new Exception("Not support");
        }

        private async Task<SendFacebookMessageReponse> SendFacebookMessage(string access_token, string psid, object message, Guid? activityId = null,
            SqlConnection conn = null)
        {
            var apiClient = new ApiClient(access_token, FacebookApiVersions.V6_0);
            var url = $"/me/messages";

            var request = (IPostRequest)ApiRequest.Create(ApiRequest.RequestType.Post, url, apiClient);
            request.AddParameter("message_type", "RESPONSE");
            request.AddParameter("recipient", JsonConvert.SerializeObject(new { id = psid }));
            request.AddParameter("message", JsonConvert.SerializeObject(message));

            var response = await request.ExecuteAsync<SendFacebookMessageReponse>();
            if (response.GetExceptions().Any())
            {
                var error = string.Join("; ", response.GetExceptions().Select(x => x.Message));
                await conn.ExecuteAsync("insert into MarketingTraces(Id,ActivityId,Exception) values (@Id,@ActivityId,@Exception)", new { Id = GuidComb.GenerateComb(), ActivityId = activityId, Exception = DateTime.Now });
                return new SendFacebookMessageReponse() { error = error };
            }
            else
            {
                var result = response.GetResult();
                await conn.ExecuteAsync("insert into MarketingTraces(Id,ActivityId,Sent,MessageId) values (@Id,@ActivityId,@Sent,@MessageId)", new { Id = GuidComb.GenerateComb(), ActivityId = activityId, Sent = DateTime.Now, MessageId = result.message_id });

                return result;
            }
        }

        public class SendFacebookMessageReponse
        {
            public string message_id { get; set; }
            public string recipient_id { get; set; }
            public string error { get; set; }
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

        public class MessageButtonTemplateJson
        {
            public MessageButtonTemplateAttachment attachment { get; set; }
        }

        public class MessageButtonTemplateAttachment
        {
            public MessageButtonTemplateAttachment()
            {
                type = "template";
            }

            public string type { get; set; }
            public MessageButtonTemplateAttachmentPayload payload { get; set; }
        }

        public class MessageButtonTemplateAttachmentPayload
        {
            public MessageButtonTemplateAttachmentPayload()
            {
                template_type = "button";
            }

            public string template_type { get; set; }
            public string text { get; set; }
            public IEnumerable<MessageButtonTemplateAttachmentPayloadButton> buttons { get; set; }
        }

        public class MessageButtonTemplateAttachmentPayloadButton
        {
            public string type { get; set; }
            public string url { get; set; }
            public string title { get; set; }
        }
    }
}
