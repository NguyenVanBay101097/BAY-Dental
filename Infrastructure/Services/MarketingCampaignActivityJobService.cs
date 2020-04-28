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

                        if (page == null)
                            return;
                      
                            var tasks = profiles.Select(x => UserprofileTags(x, activity.Id, activity.ActionType, conn)).ToList();
                            var limit = 200;
                            var offset = 0;
                            var subTasks = tasks.Skip(offset).Take(limit).ToList();
                            while (subTasks.Any())
                            {
                                await Task.WhenAll(subTasks);
                                offset += limit;
                                subTasks = tasks.Skip(offset).Take(limit).ToList();
                            }
                        //}
                        //else if (activity.ActionType == "remove_tags")
                        //{

                        //    var tagIds = GetTagRels(activityId, conn).Select(x => x.TagId).ToList();
                        //    if (tagIds == null)
                        //        return;
                        //    var tasks = profiles.Select(x => RemoveUserprofileTags(x.Id, tagIds, conn)).ToList();
                        //    var limit = 200;
                        //    var offset = 0;
                        //    var subTasks = tasks.Skip(offset).Take(limit).ToList();
                        //    while (subTasks.Any())
                        //    {
                        //        await Task.WhenAll(subTasks);
                        //        offset += limit;
                        //        subTasks = tasks.Skip(offset).Take(limit).ToList();
                        //    }

                        //}
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
            ISpecification<FacebookUserProfile> profileSpec = new InitialSpecification<FacebookUserProfile>(x => x.FbPageId == pageId);
            if (!string.IsNullOrEmpty(self.AudienceFilter))
            {
                var filter = JsonConvert.DeserializeObject<SimpleFilter>(self.AudienceFilter);
                if (filter.items.Any())
                {
                    var itemSpecs = new List<ISpecification<FacebookUserProfile>>();
                    foreach (var item in filter.items)
                    {
                        var parameter = Expression.Parameter(typeof(FacebookUserProfile), "x");
                        var expression = ToLamdaExpression(item, parameter);

                        var predicateExpression = Expression.Lambda<Func<FacebookUserProfile, bool>>(expression, parameter);
                        itemSpecs.Add(new InitialSpecification<FacebookUserProfile>(predicateExpression));
                    }

                    if (filter.type == "and")
                    {
                        ISpecification<FacebookUserProfile> tmp = new InitialSpecification<FacebookUserProfile>(x => true);
                        foreach (var spec in itemSpecs)
                            tmp = tmp.And(spec);
                        profileSpec = profileSpec.And(tmp);
                    }
                    else
                    {
                        ISpecification<FacebookUserProfile> tmp = new InitialSpecification<FacebookUserProfile>(x => false);
                        foreach (var spec in itemSpecs)
                            tmp = tmp.Or(spec);
                        profileSpec = profileSpec.And(tmp);
                    }
                }
            }
            var userProfiles = conn.Query<FacebookUserProfile>("" +
                             "SELECT * " +
                             "FROM FacebookUserProfiles us " +
                             "where us.FbPageId = @pageId  " +
                             "", new { pageId = pageId }).ToList();
            var query = userProfiles.AsQueryable();
            var result = query.Where(profileSpec.AsExpression()).ToList();
            return result;
        }

        private static Expression ToLamdaExpression(SimpleFilterItem item, ParameterExpression parameter)
        {
            Expression resultExpression = null;
            if (item.type == "Name" || item.type == "FirstName" || item.type == "LastName" || item.type == "Gender")
            {
                Expression left = Expression.PropertyOrField(parameter, item.type);
                Expression right = Expression.Constant(item.formula_value);
                switch (item.formula_type)
                {
                    case "contains":
                    case "doesnotcontain":
                    case "startswith":
                        var nullCheckExpression = Expression.Equal(left, Expression.Constant(null, typeof(String)));

                        if (item.formula_type == "contains" || item.formula_type == "doesnotcontain")
                        {
                            var containsMethod = typeof(String).GetMethod("Contains", new[] { typeof(String) });
                            var containsExpression = Expression.Call(left, containsMethod, right);
                            if (item.formula_type == "contains")
                                resultExpression = Expression.AndAlso(Expression.Not(nullCheckExpression), containsExpression);
                            else
                                resultExpression = Expression.AndAlso(Expression.Not(nullCheckExpression), Expression.Not(containsExpression));
                        }
                        else if (item.formula_type == "startswith")
                        {
                            var startswithMethod = typeof(String).GetMethod("StartsWith", new[] { typeof(String) });
                            var startswithExpression = Expression.Call(left, startswithMethod, right);
                            resultExpression = Expression.AndAlso(Expression.Not(nullCheckExpression), startswithExpression);
                        }
                        break;
                    case "eq":
                    case "neq":
                        var equalCheckExpression = Expression.Equal(left, right);
                        if (item.formula_type == "eq")
                            resultExpression = equalCheckExpression;
                        else
                            resultExpression = Expression.Not(equalCheckExpression);
                        break;
                    default:
                        throw new NotSupportedException(string.Format("Not support Operator {0}!", item.formula_type));
                }
            }
            else if (item.type == "Tag")
            {

                Expression tagRelsExpression = Expression.PropertyOrField(parameter, "TagRels");
                switch (item.formula_type)
                {
                    case "eq":
                    case "neq":
                        // find Any method                       
                        var generic = typeof(Queryable).GetMethods()
                               .Where(m => m.Name == "Any")
                               .Where(m => m.GetParameters().Length == 2)
                               .Single();
                        var containsMethod = generic.MakeGenericMethod(typeof(FacebookUserProfileTagRel));

                        var tagRelParameter = Expression.Parameter(typeof(FacebookUserProfileTagRel), "s");
                        Expression left = Expression.PropertyOrField(tagRelParameter, "Tag");
                        Expression left2 = Expression.PropertyOrField(left, "Name");
                        Expression right = Expression.Constant(item.formula_value);
                        Expression equalExpression = Expression.Equal(left2, right);

                        var predicate = Expression.Lambda<Func<FacebookUserProfileTagRel, bool>>(equalExpression, tagRelParameter);
                        var containsExpression = ExpressionUtils.CallAny(tagRelsExpression, predicate, "Any");
                        if (item.formula_type == "eq")
                            resultExpression = containsExpression;
                        else
                            resultExpression = Expression.Not(containsExpression);
                        break;
                    default:
                        throw new NotSupportedException(string.Format("Not support Operator {0}!", item.formula_type));
                }
            }
            else
                throw new NotSupportedException(string.Format("Not support type {0}!", item.type));

            return resultExpression;
        }

        private async Task UserprofileTags(FacebookUserProfile profile, Guid activityId, string actionType,
            SqlConnection conn = null )
        {
            var tags = conn.Query<MarketingCampaignActivityFacebookTagRel>(""
               + "SELECT * " +
                       "FROM MarketingCampaignActivityFacebookTagRels rel " +
                       "where rel.ActivityId = @id" +
                       "", new { id = activityId }).ToList();
            if (actionType == "add_tags")
            {
                if (tags.Any())
                {
                    foreach (var tag in tags)
                    {
                        if (profile.TagRels.Any(x => x.TagId == tag.TagId))
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
            else if(actionType == "remove_tags")
            {
                if (tags.Any())
                {
                    var tagids = tags.Select(x => x.TagId).ToList();
                    await conn.ExecuteAsync("DELETE FacebookUserProfileTagRels WHERE UserProfileId = @UserProfileId AND TagId in @TagIds ", new { UserProfileId = profile.Id, TagIds = tagids });
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
