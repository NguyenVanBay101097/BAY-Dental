﻿using ApplicationCore.Entities;
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

                        if (page == null)
                            return;
                        if (activity.ActionType == "add_tags")
                        {

                            var tasks = profiles.Select(x => AddUserprofileTags(x.Id, activity.Id, conn)).ToList();
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
                        else if (activity.ActionType == "remove_tags")
                        {

                            var tagIds = GetTagRels(activityId, conn).Select(x => x.TagId).ToList();
                            if (tagIds == null)
                                return;
                            var tasks = profiles.Select(x => RemoveUserprofileTags(x.Id, tagIds, conn)).ToList();
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
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private static IEnumerable<UserProfile> GetUserProfiles(Guid pageId, MarketingCampaignActivity activity,
            SqlConnection conn = null)
        {
            var builder = new SqlBuilder();
            var sqltemplate = builder.AddTemplate("SELECT /**select**/ FROM FacebookUserProfiles us /**leftjoin**/ /**where**/ /**orderby**/ ");
            var profiles = conn.Query<UserProfile>("" +
                              "SELECT us.id , us.PSID , us.Name " +
                              "FROM FacebookUserProfiles us " +
                              "where us.FbPageId = @pageId  " +
                              "", new { pageId = pageId }).ToList();
            builder.Select("us.id , us.PSID , us.Name ");
            builder.Where("us.FbPageId = @PageId ", new { PageId = pageId });
            var iUserprofiles = conn.Query<UserProfile>(sqltemplate.RawSql,sqltemplate.Parameters).ToList();
            var reltags = GetTagRels(activity.Id, conn).Select(x => x.TagId).ToList();
            if (activity.ActivityType == "message")
            {
                // where type == "Tag"
                
                if (reltags.Count > 0)
                {
                    builder.LeftJoin("FacebookUserProfileTagRels as rel  On rel.UserProfileId = us.Id");
                    builder.Where("rel.TagId in @Tagids", new { Tagids = reltags });
                    builder.GroupBy("us.id, us.PSID, us.Name");
                    iUserprofiles = conn.Query<UserProfile>(sqltemplate.RawSql, sqltemplate.Parameters).ToList();

                }

                // where type == "Gender"
                //--TODO--//

                // where type == "FirstName"

                //--TODO--//

                // where type == "LastName"

                //--TODO--//   
            }

            if (activity.ActionType == "add_tags")
            {
                iUserprofiles = conn.Query<UserProfile>(sqltemplate.RawSql, sqltemplate.Parameters).ToList();
            }
            if (activity.ActionType == "remove_tags")
            {
                if (reltags.Count > 0)
                {
                    builder.LeftJoin("FacebookUserProfileTagRels as rel  On rel.UserProfileId = us.Id");
                    builder.Where("rel.TagId in @Tagids", new { Tagids = reltags });
                    builder.GroupBy("us.id, us.PSID, us.Name");
                    iUserprofiles = conn.Query<UserProfile>(sqltemplate.RawSql, sqltemplate.Parameters).ToList();

                }
            }
            if (activity.TriggerType == "message_open")
            {
                iUserprofiles = conn.Query<UserProfile>("" +
                             "SELECT us.id , us.PSID , us.Name " +
                            "FROM MarketingTraces m " +
                            "Left join FacebookUserProfiles as us on us.Id = m.UserProfileId " +
                            "where m.ActivityId = @activityId AND m.[Read] is not null " +
                            "", new { activityId = activity.ParentId }).ToList();

            }

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

        private async Task AddUserprofileTags(Guid profileId, Guid activityId,
            SqlConnection conn = null)
        {
            var userprofile = conn.QueryFirstOrDefaultAsync<FacebookUserProfile>("" +
                 "SELECT * " +
                              "FROM FacebookUserProfiles  " +
                              "where Id = @Id  " +
                              "", new { Id = profileId }).Result;
            var tags = conn.Query<MarketingCampaignActivityFacebookTagRel>(""
                + "SELECT * " +
                        "FROM MarketingCampaignActivityFacebookTagRels rel " +
                        "where rel.ActivityId = @id" +
                        "", new { id = activityId }).ToList();
            if (tags.Any())
            {
                foreach (var tag in tags)
                {
                    if (userprofile.TagRels.Any(x => x.TagId == tag.TagId))
                        continue;
                    userprofile.TagRels.Add(new FacebookUserProfileTagRel
                    {
                        TagId = tag.TagId,
                        UserProfileId = userprofile.Id
                    });

                }
                string processQuery = "INSERT INTO FacebookUserProfileTagRels VALUES(@UserProfileId,@TagId)";
                await conn.ExecuteAsync(processQuery, userprofile.TagRels);

            }

        }
        public async Task RemoveUserprofileTags(Guid userProfileId, IEnumerable<Guid> tagIds,
            SqlConnection conn = null)
        {

            await conn.ExecuteAsync("DELETE FacebookUserProfileTagRels WHERE UserProfileId = @UserProfileId AND TagId in @TagIds ", new { UserProfileId = userProfileId, TagIds = tagIds });


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

        private async Task SendFacebookMessage(string access_token, object message, UserProfile profile, Guid? activityId = null,
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
