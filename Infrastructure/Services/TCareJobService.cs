using ApplicationCore.Entities;
using Dapper;
using Facebook.ApiClient.ApiEngine;
using Facebook.ApiClient.Constants;
using Facebook.ApiClient.Interfaces;
using Hangfire;
using Hangfire.States;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyERP.Utilities;
using Newtonsoft.Json;
using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Umbraco.Web.Models.ContentEditing;
using ZaloDotNetSDK;
using ZaloDotNetSDK.oa;

namespace Infrastructure.Services
{
    public class TCareJobService : ITCareJobService
    {
        private readonly ConnectionStrings _connectionStrings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFacebookMessageSender _fbMessageSender;
        public TCareJobService(IOptions<ConnectionStrings> connectionStrings, IFacebookMessageSender fbMessageSender, IHttpContextAccessor httpContextAccessor)
        {
            _connectionStrings = connectionStrings?.Value;
            _fbMessageSender = fbMessageSender;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Run(string db, Guid campaignId)
        {
            //await TCareTakeMessage(db);
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.CatalogConnection);
            if (db != "localhost")
                builder["Database"] = $"TMTDentalCatalogDb__{db}";
            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    conn.Open();

                    var messages = await conn.QueryAsync<TCareMessage>("SELECT * FROM TCareMessages WHERE State = 'waiting'");
                    if (messages.Count() == 0)
                        return;

                    //var date = DateTime.UtcNow;
                    //var campaign = conn.Query<TCareCampaign>("SELECT * FROM TCareCampaigns WHERE Id = @id", new { id = campaignId }).FirstOrDefault();

                    //var partner_ids = await SearchPartnerIdsV2(campaign.GraphXml, conn);
                    //if (partner_ids == null)
                    //    return;

                    //XmlSerializer serializer = new XmlSerializer(typeof(MxGraphModel));
                    //MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(campaign.GraphXml));
                    //MxGraphModel CampaignXML = (MxGraphModel)serializer.Deserialize(memStream);
                    //var partner_ids = SearchPartnerIds(CampaignXML.Root.Rule.Condition, CampaignXML.Root.Rule.Logic, conn);
                    foreach (var message in messages)
                        BackgroundJob.Enqueue(() => SendMessgeToPage(db, message));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public async Task TCareTakeMessage(string db)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.CatalogConnection);
            if (db != "localhost")
                builder["Database"] = $"TMTDentalCatalogDb__{db}";
            FacebookPage channel = null;
            string channelType = "";
            FacebookUserProfile facebookUserProfile = null;
            List<TCareCampaign> listCampaigns = new List<TCareCampaign>();
            Partner partner = null;
            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    conn.Open();
                    var listScenarios = await conn.QueryAsync<TCareScenario>("SELECT * FROM TCareScenarios");
                    foreach (var scenario in listScenarios)
                    {
                        channelType = scenario.ChannalType;
                        channel = await GetChannel(conn, scenario.ChannelSocialId.Value);
                        if (channel == null)
                            continue;
                        listCampaigns = (await GetListCampaign(conn, scenario.Id)).ToList();
                        foreach (var campaign in listCampaigns)
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(MxGraphModel));
                            MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(campaign.GraphXml));
                            MxGraphModel resultingMessage = (MxGraphModel)serializer.Deserialize(memStream);
                            TCareMessaging messaging = new TCareMessaging();
                            var sequence = resultingMessage.Root.Sequence;
                            var rule = resultingMessage.Root.Rule;
                            if (sequence == null || rule == null)
                                continue;

                            if (string.IsNullOrEmpty(sequence.Content))
                                continue;


                            var partner_ids = await SearchPartnerIdsV2(campaign.GraphXml, conn);
                            if (partner_ids == null)
                                continue;
                            messaging.Id = GuidComb.GenerateComb();
                            messaging.Date = DateTime.Now;
                            messaging.TCareCampaignId = campaign.Id;
                            messaging.Content = sequence.Content;
                            await CreateMessaging(conn, messaging);

                            int number = 0;
                            foreach (var partner_id in partner_ids)
                            {
                                var facebookUserProfiles = GetFacebookUserProfilesByPartnerId(conn, partner_id);
                                if (facebookUserProfiles.Count() <= 0)
                                    continue;
                                partner = await GetPartner(conn, partner_id);
                                var messageContent = PersonalizedPartner(partner, channel, sequence, conn);
                                facebookUserProfile = facebookUserProfiles.Where(x => x.FbPageId == channel.Id).FirstOrDefault();
                                if (facebookUserProfile == null)
                                    continue;
                                TCareMessage tcareMessage = new TCareMessage()
                                {
                                    ProfilePartnerId = facebookUserProfile.Id,
                                    ChannelSocicalId = channel.Id,
                                    CampaignId = campaign.Id,
                                    PartnerId = partner_id,
                                    MessageContent = messageContent,
                                    TCareMessagingId = messaging.Id,
                                    State = "waiting"
                                };
                                CreateMessage(conn, tcareMessage);
                                number++;
                            }
                            await UpdateNumberPartner(conn, messaging.Id, number);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
        }

        public async Task CreateMessaging(SqlConnection conn, TCareMessaging tcareMessaging)
        {
            await conn.ExecuteAsync("insert into TCareMessagings" +
                 "(Id ,Content , TCareCampaignId, CountPartner, Date) " +
                 "Values (@Id, @Content, @TCareCampaignId, @CountPartner, @Date)",
                 new
                 {
                     Id = tcareMessaging.Id,
                     Content = tcareMessaging.Content,
                     TCareCampaignId = tcareMessaging.TCareCampaignId,
                     CountPartner = tcareMessaging.CountPartner,
                     Date = tcareMessaging.Date,
                 });
        }

        public async Task UpdateNumberPartner(SqlConnection conn, Guid id, int countPartner)
        {
            await conn.ExecuteAsync("UPDATE TCareMessagings SET CountPartner = @CountPartner WHERE Id = @Id",
                 new
                 {
                     Id = id,
                     CountPartner = countPartner
                 });
        }

        public void CreateMessage(SqlConnection conn, TCareMessage tcareMessage)
        {
            var id = GuidComb.GenerateComb();
            conn.Execute("insert into TCareMessages" +
                "(Id,ProfilePartnerId, ChannelSocicalId, CampaignId, PartnerId, MessageContent ,State ,TCareMessagingId) " +
                "Values (@Id,@ProfilePartnerId,@ChannelSocicalId,@CampaignId,@PartnerId,@MessageContent,@State,@TCareMessagingId)",
                new
                {
                    Id = id,
                    ProfilePartnerId = tcareMessage.ProfilePartnerId,
                    ChannelSocicalId = tcareMessage.ChannelSocicalId,
                    CampaignId = tcareMessage.CampaignId,
                    PartnerId = tcareMessage.PartnerId,
                    MessageContent = tcareMessage.MessageContent,
                    State = tcareMessage.State,
                    TCareMessagingId = tcareMessage.TCareMessagingId
                });
        }

        public void UpdateMessage(SqlConnection conn, Guid id, Guid? messagingTraceId)
        {
            conn.Execute("UPDATE TCareMessages SET State = 'success', TCareMessagingTraceId = @TCareMessagingTraceId WHERE Id = @Id",
                new
                {
                    Id = id,
                    TCareMessagingTraceId = messagingTraceId
                });
        }

        public async Task<Partner> GetPartner(SqlConnection conn, Guid id)
        {
            return (await conn.QueryAsync<Partner>("SELECT * FROM Partners where Id = @id", new { id = id })).FirstOrDefault();
        }

        public async Task<IEnumerable<TCareCampaign>> GetListCampaign(SqlConnection conn, Guid scenarioId)
        {
            return await conn.QueryAsync<TCareCampaign>("SELECT * FROM TCareCampaigns where TCareScenarioId = @ScenarioId AND State = 'running'", new { ScenarioId = scenarioId });
        }

        public async Task<FacebookPage> GetChannel(SqlConnection conn, Guid id)
        {
            return (await conn.QueryAsync<FacebookPage>("SELECT * FROM FacebookPages where Id = @id", new { id = id })).FirstOrDefault();
        }

        public async Task SendMessgeToPage(string db, TCareMessage mess)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.CatalogConnection);
            if (db != "localhost")
                builder["Database"] = $"TMTDentalCatalogDb__{db}";
            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    conn.Open();
                    var partner = await GetPartner(conn, mess.PartnerId.Value);
                    var partnerProfile = await GetPartnerProfile(conn, mess.ProfilePartnerId.Value);
                    var campaign = await GetCampaign(conn, mess.CampaignId.Value);
                    var channelSocial = await GetChannel(conn, mess.ChannelSocicalId.Value);
                    await SendMessagePage(conn, campaign, mess, partnerProfile, db, channelSocial);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<TCareCampaign> GetCampaign(SqlConnection conn, Guid id)
        {
            var campaign = (await conn.QueryAsync<TCareCampaign>("SELECT * FROM TCareCampaigns WHERE Id = @Id", new { Id = id })).FirstOrDefault();
            return campaign;
        }

        public async Task<FacebookUserProfile> GetPartnerProfile(SqlConnection conn, Guid id)
        {
            var partnerProfile = (await conn.QueryAsync<FacebookUserProfile>("SELECT * FROM FacebookUserProfiles WHERE Id = @Id", new { Id = id })).FirstOrDefault();
            return partnerProfile;
        }

        public void SetTagForPartner(Guid partnerId, Guid TagId, SqlConnection conn)
        {

            var model = conn.Query<PartnerPartnerCategoryRel>
                ("SELECT * FROM PartnerPartnerCategoryRel " +
                "WHERE PartnerId = @partnerId AND CategoryId = @categoryId", new { partnerId = partnerId, categoryId = TagId });
            if (model.Count() == 0)
                conn.Query<PartnerPartnerCategoryRel>
                    ("INSERT INTO PartnerPartnerCategoryRel(PartnerId, CategoryId)" +
                    " VALUES(CONVERT(uniqueidentifier, @partnerId) , CONVERT(uniqueidentifier, @tagId))", new { partnerId = partnerId.ToString(), tagId = TagId.ToString() });

        }

        private async Task SendMessagePage(SqlConnection conn, TCareCampaign campaign, TCareMessage mess, FacebookUserProfile profile, string db, FacebookPage channelSocial)
        {
            bool check = false;
            var trace_Id = GuidComb.GenerateComb();
            await conn.ExecuteAsync("" +
                "insert into TCareMessingTraces" +
                "(Id,TCareCampaignId,PSID,PartnerId,Type,ChannelSocialId,DateCreated,LastUpdated,TCareMessagingId) " +
                "Values (@Id,@TCareCampaignId,@PSID,@PartnerId,@Type,@ChannelSocialId,@DateCreated,@LastUpdated,@TCareMessagingId)",
                new
                {
                    Id = trace_Id,
                    TCareCampaignId = campaign.Id,
                    PSID = profile.PSID,
                    PartnerId = profile.PartnerId,
                    Type = channelSocial.Type,
                    ChannelSocialId = channelSocial.Id,
                    DateCreated = DateTime.Now,
                    LastUpdated = DateTime.Now,
                    TCareMessagingId = mess.TCareMessagingId

                });
            switch (channelSocial.Type)
            {
                case "facebook":
                    check = await SendMessagePageFacebook(profile, mess.MessageContent, db, channelSocial, conn, trace_Id);
                    if (check && campaign.TagId.HasValue)
                    {
                        SetTagForPartner(profile.PartnerId.Value, campaign.TagId.Value, conn);
                        UpdateMessage(conn, mess.Id, trace_Id);
                    }
                    break;
                case "zalo":
                    check = await SendMessagePageZalo(profile, mess.MessageContent, db, channelSocial, conn, trace_Id);
                    if (check && campaign.TagId.HasValue)
                    {
                        SetTagForPartner(profile.PartnerId.Value, campaign.TagId.Value, conn);
                        UpdateMessage(conn, mess.Id, trace_Id);
                    }
                    break;
                default:
                    break;
            }
        }

        private async Task<bool> SendMessagePageZalo(FacebookUserProfile profile, string messageContent, string db, FacebookPage channelSocial, SqlConnection conn, Guid trace_Id)
        {
            var zaloClient = new ZaloClient(channelSocial.PageAccesstoken);
            var sendResult = zaloClient.sendTextMessageToUserId(profile.PSID, messageContent).ToObject<SendMessageZaloResponse>();
            if (sendResult.error != 0)
            {
                await conn.ExecuteAsync("update TCareMessingTraces set Exception=@exception, Error=@error where Id=@id", new { exception = DateTime.Now, error = sendResult.error, id = trace_Id });
                return false;
            }
            else
            {
                await conn.ExecuteAsync("update TCareMessingTraces set MessageId=@messageId, Sent=@sent where Id=@id", new { messageId = sendResult.data.message_id, sent = DateTime.Now, id = trace_Id });
                return true;
            }
        }

        private async Task<bool> SendMessagePageFacebook(FacebookUserProfile profile, string messageContent, string db, FacebookPage channelSocial, SqlConnection conn, Guid trace_Id)
        {
            var sendResult = await _fbMessageSender.SendMessageTCareTextAsync(messageContent, profile.PSID, channelSocial.PageAccesstoken);
            if (!string.IsNullOrEmpty(sendResult.error))
            {
                await conn.ExecuteAsync("update TCareMessingTraces set Exception=@exception, Error=@error where Id=@id", new { exception = DateTime.Now, error = sendResult.error, id = trace_Id });
                return false;
            }
            else
            {
                await conn.ExecuteAsync("update TCareMessingTraces set MessageId=@messageId where Id=@id", new { messageId = sendResult.message_id, id = trace_Id });
                BackgroundJob.Enqueue(() => GetInfoMessageFBApi(db, trace_Id, channelSocial.PageAccesstoken, sendResult.message_id));
                return true;
            }
        }

        private string PersonalizedPartner(Partner partner, FacebookPage channelSocial, Sequence sequence, SqlConnection conn)
        {
            var messageContent = sequence.Content.Replace("@ten_khach_hang", partner.Name.Split(' ').Last()).Replace("@fullname_khach_hang", partner.Name).Replace("@ten_page", channelSocial.PageName);
            if (messageContent.Contains("@danh_xung_khach_hang"))
            {
                PartnerTitle partnerTitle = null;
                if (partner.TitleId.HasValue)
                {
                    partnerTitle = conn.Query<PartnerTitle>("" +
                        "SELECT * " +
                        "FROM PartnerTitles " +
                        "where Id = @id" +
                        "", new { id = partner.TitleId }).FirstOrDefault();
                }

                messageContent = messageContent.Replace("@danh_xung_khach_hang", partnerTitle != null ? partnerTitle.Name.ToLower() : "");
            }
            return messageContent;
        }

        //private string PersonalizedPartner(Partner partner, FacebookPage channelSocial, Sequence sequence, SqlConnection conn)
        //{
        //    var messageContent = sequence.Content.Replace("@ten_khach_hang", partner.Name.Split(' ').Last()).Replace("@fullname_khach_hang", partner.Name).Replace("@ten_page", channelSocial.PageName);
        //    if (messageContent.Contains("@danh_xung_khach_hang"))
        //    {
        //        PartnerTitle partnerTitle = null;
        //        if (partner.TitleId.HasValue)
        //        {
        //            partnerTitle = conn.Query<PartnerTitle>("" +
        //                "SELECT * " +
        //                "FROM PartnerTitles " +
        //                "where Id = @id" +
        //                "", new { id = partner.TitleId }).FirstOrDefault();
        //        }

        //        messageContent = messageContent.Replace("@danh_xung_khach_hang", partnerTitle != null ? partnerTitle.Name.ToLower() : "");
        //    }
        //    return messageContent;
        //}

        private static IEnumerable<FacebookUserProfile> GetFacebookUserProfilesByPartnerId(SqlConnection conn, Guid partId)
        {
            var userProfile = conn.Query<FacebookUserProfile>("select * from FacebookUserProfiles where PartnerId = @PartnerId",
               new { PartnerId = partId }).ToList();
            return userProfile;
        }

        [AutomaticRetry]
        public async Task GetInfoMessageFBApi(string db, Guid trace_id, string access_token, string message_id)
        {
            //lấy thông tin message từ facebook api rồi cập nhật cho message log
            var apiClient = new ApiClient(access_token, FacebookApiVersions.V6_0);
            var url = $"/{message_id}";

            var request = (IGetRequest)ApiRequest.Create(ApiRequest.RequestType.Get, url, apiClient);
            request.AddQueryParameter("fields", "id,message,created_time");
            var response = await request.ExecuteAsync<FacebookMessageApiInfoReponse>();
            if (!response.GetExceptions().Any())
            {
                var result = response.GetResult();

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.CatalogConnection);
                if (db != "localhost")
                    builder["Database"] = $"TMTDentalCatalogDb__{db}";

                using (var conn = new SqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    await conn.ExecuteAsync("update TCareMessingTraces set Sent=@sent where Id=@id", new { Id = trace_id, Sent = result.created_time.ToLocalTime() });
                }
            }
        }

        private async Task<IEnumerable<Guid>> SearchPartnerIdsV2(string graphXml, SqlConnection conn)
        {
            string logic = "and";
            var conditions = new List<object>();
            var conditionPartnerIds = new List<IList<Guid>>();
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(graphXml)))
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Async = true;
                using (XmlReader reader = XmlReader.Create(stream, settings))
                {
                    while (await reader.ReadAsync())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            switch (reader.Name)
                            {
                                case "rule":
                                    {
                                        logic = reader.GetAttribute("logic");
                                        var subReader = reader.ReadSubtree();
                                        while (subReader.ReadToFollowing("condition"))
                                        {
                                            var type = subReader.GetAttribute("type");
                                            var name = subReader.GetAttribute("name");
                                            if (type == "categPartner")
                                            {
                                                var op = subReader.GetAttribute("op");
                                                Guid tagId;
                                                Guid.TryParse(subReader.GetAttribute("tagId"), out tagId);
                                                var cond = new PartnerCategoryCondition() { Name = name, Type = type, Op = op, TagId = tagId };
                                                conditions.Add(cond);
                                            }
                                            else if (type == "birthday")
                                            {
                                                int day = 0;
                                                int.TryParse(subReader.GetAttribute("day"), out day);
                                                var cond = new PartnerBirthdayCondition() { Name = name, Type = type, Day = day };
                                                conditions.Add(cond);
                                            }
                                            else if (type == "lastSaleOrder")
                                            {
                                                int day = 0;
                                                int.TryParse(subReader.GetAttribute("day"), out day);
                                                var cond = new LastSaleOrderCondition() { Name = name, Type = type, Day = day };
                                                conditions.Add(cond);
                                            }
                                            else if (type == "lastExamination")
                                            {
                                                int day = 0;
                                                int.TryParse(subReader.GetAttribute("day"), out day);
                                                var cond = new LastDotKhamDateCondition() { Name = name, Type = type, Day = day };
                                                conditions.Add(cond);
                                            }
                                            else if (type == "lastAppointment")
                                            {
                                                int day = 0;
                                                int.TryParse(subReader.GetAttribute("day"), out day);
                                                var cond = new LastAppointmentDateCondition() { Name = name, Type = type, Day = day };
                                                conditions.Add(cond);
                                            }
                                        }
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                    }
                }
            }

            foreach (var condition in conditions)
            {
                var type = condition.GetType().GetProperty("Type").GetValue(condition, null);
                switch (type)
                {
                    case "categPartner":
                        {
                            var cond = (PartnerCategoryCondition)condition;
                            var sqlOp = cond.Op == "not_contains" ? "NOT EXISTS" : "EXISTS";
                            var searchPartnerIds = conn.Query<Guid>("" +
                                                       "select p.Id " +
                                                       "from Partners p " +
                                                       $"where p.Customer = 1 and {sqlOp} " +
                                                       "(select 1 from PartnerPartnerCategoryRel p0 where p0.PartnerId = p.Id and p0.CategoryId = @categId)", new { categId = cond.TagId }).ToList();
                            conditionPartnerIds.Add(searchPartnerIds);
                            break;
                        }
                    case "birthday":
                        {
                            var today = DateTime.Today;
                            var cond = (PartnerBirthdayCondition)condition;
                            var date = today.AddDays(cond.Day);
                            var searchPartnerIds = conn.Query<Guid>("" +
                                           "Select pn.Id " +
                                           "From Partners pn " +
                                           "Where pn.Customer = 1 and pn.BirthDay = @day AND pn.BirthMonth = @month ", new { day = date.Day, month = date.Month }
                                           ).ToList();
                            conditionPartnerIds.Add(searchPartnerIds);
                            break;
                        }
                    case "lastSaleOrder":
                        {
                            var cond = (LastSaleOrderCondition)condition;
                            var searchPartnerIds = conn.Query<Guid>("" +
                                                    "Select pn.Id From Partners pn " +
                                                    "Left join SaleOrders sale ON sale.PartnerId = pn.Id " +
                                                    "Where pn.Customer = 1 and sale.State in ('sale','done') " +
                                                    "Group by pn.Id " +
                                                    "Having (Max(sale.DateOrder) < DATEADD(day, -@number, GETDATE())) ", new { number = cond.Day }).ToList();
                            conditionPartnerIds.Add(searchPartnerIds);
                            break;
                        }
                    case "lastExamination":
                        {
                            var cond = (LastDotKhamDateCondition)condition;
                            var searchPartnerIds = conn.Query<Guid>("" +
                                     "Select pn.Id From Partners pn " +
                                     "Left join DotKhams dk ON dk.PartnerId = pn.Id " +
                                     "Where pn.Customer = 1 " +
                                     "Group by pn.Id " +
                                     "Having (Max(dk.Date) <= DATEADD(day, -@number, GETDATE())) ", new { number = cond.Day }).ToList();
                            conditionPartnerIds.Add(searchPartnerIds);
                            break;
                        }
                    case "lastAppointment":
                        {
                            var cond = (LastAppointmentDateCondition)condition;
                            var searchPartnerIds = conn.Query<Guid>("" +
                                 "Select pn.Id From Partners pn " +
                                 "Left join Appointments am ON am.PartnerId = pn.Id " +
                                 "Where pn.Customer = 1 " +
                                 "Group by pn.Id " +
                                 "Having (Max(CONVERT(date, am.Date)) = DATEADD(day, @number, CONVERT(date, GETDATE()))) ", new { number = cond.Day }).ToList();
                            conditionPartnerIds.Add(searchPartnerIds);
                            break;
                        }
                    default:
                        break;
                }
            }


            IEnumerable<Guid> result = null;
            foreach (var item in conditionPartnerIds)
            {
                if (result == null)
                {
                    result = item;
                    continue;
                }

                if (logic == "or")
                    result = result.Union(item);
                else
                    result = result.Intersect(item);
            }

            return result;
        }
    }

    public class PartnerCategoryCondition
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string Op { get; set; }

        public Guid TagId { get; set; }
    }

    public class PartnerBirthdayCondition
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public int Day { get; set; }
    }

    public class LastSaleOrderCondition
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public int Day { get; set; }
    }

    public class LastDotKhamDateCondition
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public int Day { get; set; }
    }

    public class LastAppointmentDateCondition
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public int Day { get; set; }
    }

    public class PartnerSendMessageResult
    {
        public string ZaloId { get; set; }
        public string Content { get; set; }
    }

    public class SendMessageZaloResponse
    {
        public string message { get; set; }
        public int error { get; set; }
        public SendMessageZaloData data { get; set; }
    }

    public class SendMessageZaloData
    {
        public string message_id { get; set; }
        public string user_id { get; set; }
    }

    public class RulePartnerIds
    {
        public List<Guid> Ids { get; set; }
    }

    public class XmlReadData
    {
    }

    public class XmlReadRuleData
    {

    }

}
