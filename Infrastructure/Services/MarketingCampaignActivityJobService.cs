using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
using Dapper;
using Facebook.ApiClient.ApiEngine;
using Facebook.ApiClient.Constants;
using Facebook.ApiClient.Interfaces;
using Hangfire;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyERP.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;
using ZaloDotNetSDK;

namespace Infrastructure.Services
{
    public class MarketingCampaignActivityJobService : IMarketingCampaignActivityJobService
    {
        private readonly ConnectionStrings _connectionStrings;
        private readonly IFacebookMessageSender _fbMessageSender;
        private readonly IFacebookUserProfileService _fbUserProfile;
        private readonly IMapper _mapper;
        public MarketingCampaignActivityJobService(IOptions<ConnectionStrings> connectionStrings, IFacebookMessageSender fbMessageSender, IFacebookUserProfileService fbUserProfile, IMapper mapper)
        {
            _connectionStrings = connectionStrings?.Value;
            _fbMessageSender = fbMessageSender;
            _fbUserProfile = fbUserProfile;
            _mapper = mapper;


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
                    var page = conn.Query<FacebookPage>("" +
                        "SELECT * " +
                        "FROM FacebookPages " +
                        "where Id = @id" +
                        "", new { id = campaign.FacebookPageId }).FirstOrDefault();

                    if (page == null)
                        return;
                    var profiles = GetUserProfiles(page.Id, activity, conn);
                    if (profiles == null)
                        return;
                    if (activity.ActivityType == "message")
                    {
                        var message_obj = GetMessageForSendApi(conn, activity.MessageId.Value);
                        if (!activity.MessageId.HasValue)
                            return;
                        var tasks = profiles.Select(x => SendFacebookMessage(page.PageAccesstoken, message_obj, x, activity.Id, conn)).ToList();
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
                    else if (activity.ActivityType == "action")
                    {

                        var tags = conn.Query<MarketingCampaignActivityFacebookTagRel>(""
                        + "SELECT * " +
                       "FROM MarketingCampaignActivityFacebookTagRels rel " +
                       "where rel.ActivityId = @id" +
                       "", new { id = activityId }).ToList();  

                        // Get Tagrels in List Userprofiles
                        var tagss = conn.Query<FacebookUserProfileTagRel>("" +
                                        "Select rel.* " +
                                        "From FacebookUserProfileTagRels rel " +
                                        "Where rel.UserProfileId in @Id ", new { Id = profiles.Select(x => x.Id).ToList() }
                                        ).ToList();
                        if (tags.Any())
                        {
                            if (activity.ActionType == "add_tags")
                            {
                                foreach (var profile in profiles)
                                {
                                    foreach (var tag in tags)
                                    {
                                        if (tagss.Any(x => x.TagId == tag.TagId))
                                            continue;
                                        profile.TagRels.Add(new FacebookUserProfileTagRel
                                        {
                                            TagId = tag.TagId,
                                            UserProfileId = profile.Id
                                        });
                                    }

                                    string processQuery = "INSERT INTO FacebookUserProfileTagRels VALUES(@UserProfileId,@TagId)";
                                    await conn.ExecuteAsync(processQuery, profile.TagRels);
                                }


                            }
                            else if (activity.ActionType == "remove_tags")
                            {
                                var toRemove = tagss.Where(x => tags.Any(s => s.TagId == x.TagId)).ToList();
                                if (toRemove.Any())
                                {
                                    string processQueryDelete = "DELETE FacebookUserProfileTagRels WHERE UserProfileId in @UserProfileId And TagId in @TagId";
                                    await conn.ExecuteAsync(processQueryDelete, new { UserProfileId = toRemove.Select(x => x.UserProfileId).ToList(), TagId = toRemove.Select(x => x.TagId).ToList() });
                                }
                            }
                        }

                       

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }



        private static IEnumerable<FacebookUserProfile> GetUserProfiles(Guid pageId, MarketingCampaignActivity activity,
            SqlConnection conn = null)
        {
            var builder = new SqlBuilder();
            var sqltemplate = builder.AddTemplate("SELECT /**select**/ FROM FacebookUserProfiles us /**leftjoin**/ /**where**/ /**orderby**/ ");

            builder.Select("us.id , us.PSID , us.Name ");
            builder.Where("us.FbPageId = @PageId ", new { PageId = pageId });
            var iUserprofiles = conn.Query<UserProfile>(sqltemplate.RawSql, sqltemplate.Parameters).ToList();
            var reltags = GetTagRels(activity.Id, conn).Select(x => x.TagId).ToList();
            if ( activity.ActionType == "remove_tags")
            {
                if (reltags.Count > 0)
                {
                    builder.LeftJoin("FacebookUserProfileTagRels as rel  On rel.UserProfileId = us.Id");
                    builder.Where("rel.TagId in @Tagids", new { Tagids = reltags });
                    builder.GroupBy("us.id, us.PSID, us.Name");
                    iUserprofiles = conn.Query<UserProfile>(sqltemplate.RawSql, sqltemplate.Parameters).ToList();

                }
            }
            if ( activity.TriggerType == "message_open")
            {
                iUserprofiles = conn.Query<UserProfile>("" +
                             "SELECT us.id , us.PSID , us.Name " +
                            "FROM MarketingTraces m " +
                            "Left join FacebookUserProfiles as us on us.Id = m.UserProfileId " +
                            "where m.ActivityId = @activityId AND m.[Read] is not null " +
                            "", new { activityId = activity.ParentId }).ToList();

            }
            // lấy danh sách userprofiles theo filter
            var profiles = GetProfilesActivity(activity, pageId, conn);        
            var lstUserProfile = profiles.Where(x => iUserprofiles.Any(s => s.Id == x.Id)).ToList();


            return lstUserProfile;

        }

        private static IEnumerable<MarketingCampaignActivityFacebookTagRel> GetTagRels(Guid activityId, SqlConnection conn = null)
        {

            var tags = conn.Query<MarketingCampaignActivityFacebookTagRel>(""
               + "SELECT * " +
                       "FROM MarketingCampaignActivityFacebookTagRels rel " +
                       "where rel.ActivityId = @id" +
                       "", new { id = activityId }).ToList();
            return tags;
        }

        private static IEnumerable<FacebookUserProfile> GetProfilesActivity(MarketingCampaignActivity self, Guid pageId, SqlConnection conn = null)
        {
            //Lấy ra những profiles sẽ gửi message
            var builder = new SqlBuilder();
            var sqltemplate = builder.AddTemplate("SELECT /**select**/ FROM FacebookUserProfiles us /**leftjoin**/ /**where**/ /**orderby**/ ");
            builder.Select("us.* ");
            builder.Where("us.FbPageId = @pageId ", new { pageId = pageId });

            if (!string.IsNullOrEmpty(self.AudienceFilter))
            {
                var filter = JsonConvert.DeserializeObject<SimpleFilter>(self.AudienceFilter);
                if (filter.items.Any())
                {

                    var lst = new Dictionary<string, string>();
                    lst.Add("contains", "Like");
                    lst.Add("doesnotcontain", "Not Like");
                    lst.Add("eq", "=");
                    lst.Add("neq", "!=");
                    lst.Add("startswith", "Like");
                    foreach (var item in filter.items)
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        foreach (var kvp in lst.Where(x => x.Key == item.formula_type))
                        {
                            if (item.type == "Name" || item.type == "FirstName" || item.type == "LastName" || item.type == "Gender")
                            {

                                switch (kvp.Key)
                                {
                                    case "contains":
                                        parameters.Add($"@{item.type}", "%" + item.formula_value + "%");
                                        builder.Where($"us.{item.type} {kvp.Value} @{item.type} ", parameters);
                                        break;
                                    case "eq":
                                        parameters.Add($"@{item.type}", item.formula_value);
                                        builder.Where($"us.{item.type} {kvp.Value} @{item.type} ", parameters);
                                        break;
                                    case "doesnotcontain":
                                        parameters.Add($"@{item.type}", "%" + item.formula_value + "%");
                                        builder.Where($"us.{item.type} {kvp.Value} @{item.type} ", parameters);
                                        break;
                                    case "neq":
                                        parameters.Add($"@{item.type}", item.formula_value);
                                        builder.Where($"us.{item.type} {kvp.Value} @{item.type} ", parameters);
                                        break;
                                    case "startswith":
                                        parameters.Add($"@{item.type}", item.formula_value + "%");
                                        builder.Where($"us.{item.type} {kvp.Value} @{item.type} ", parameters);
                                        break;


                                    default:
                                        throw new NotSupportedException(string.Format("Not support Operator {0}!", item.formula_type));
                                }
                                //}

                            }
                            else if (item.type == "Tag")
                            {
                                switch (kvp.Key)
                                {

                                    case "eq":
                                        builder.LeftJoin("FacebookUserProfileTagRels as rel  On rel.UserProfileId = us.Id ");
                                        builder.LeftJoin("FacebookTags tag ON tag.Id = rel.TagId ");
                                        builder.Where($"tag.Name {kvp.Value} @TagName ", new { TagName = item.formula_value });
                                        break;
                                    case "neq":
                                        builder.LeftJoin("FacebookUserProfileTagRels as rel  On rel.UserProfileId = us.Id ");
                                        builder.LeftJoin("FacebookTags tag ON tag.Id = rel.TagId ");
                                        builder.Where($"tag.Name {kvp.Value} @TagName ", new { TagName = item.formula_value });
                                        break;
                                    default:
                                        throw new NotSupportedException(string.Format("Not support Operator {0}!", item.formula_type));
                                }
                            }
                        }


                    }



                }
            }

            var iUserprofiles = conn.Query<FacebookUserProfile>(sqltemplate.RawSql, sqltemplate.Parameters).ToList();
            return iUserprofiles;
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
                    foreach (var button in buttons)
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

        private async Task SendFacebookMessage(string access_token, object message, FacebookUserProfile profile, Guid? activityId = null,
            SqlConnection conn = null)
        {
            var now = DateTime.Now;
            var date = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            date = date.AddSeconds(-1);
            var sendResult = await _fbMessageSender.SendMessageMarketingTextAsync(message, profile.PSID, access_token);
            if (sendResult == null)
                await conn.ExecuteAsync("insert into MarketingTraces(Id,ActivityId,Exception,UserProfileId) values (@Id,@ActivityId,@Exception,@UserProfileId)", new { Id = GuidComb.GenerateComb(), ActivityId = activityId, Exception = DateTime.Now, UserProfileId = profile.Id });
            else
                await conn.ExecuteAsync("insert into MarketingTraces(Id,ActivityId,Sent,MessageId,UserProfileId) values (@Id,@ActivityId,@Sent,@MessageId,@UserProfileId)", new { Id = GuidComb.GenerateComb(), ActivityId = activityId, Sent = DateTime.Now, MessageId = sendResult.message_id, UserProfileId = profile.Id });

        }


        public class UserProfile
        {
            public Guid Id { get; set; }
            public string PSID { get; set; }
            public string Name { get; set; }
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
