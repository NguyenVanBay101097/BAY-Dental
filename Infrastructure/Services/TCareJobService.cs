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

                    var messages = await conn.QueryAsync<TCareMessage>("SELECT * FROM TCareMessages WHERE State = 'waiting' AND CampaignId = @campaignId", new { campaignId = campaignId });
                    if (messages.Count() == 0)
                        return;

                    foreach (var message in messages)
                        BackgroundJob.Enqueue(() => SendMessgeToPage(db, message));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public async Task RunJob2Messages(string db)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.CatalogConnection);
            if (db != "localhost")
                builder["Database"] = $"TMTDentalCatalogDb__{db}";

            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    conn.Open();

                    //tìm danh sách các chiến dịch đang active
                    var messagings = await conn.QueryAsync<TCareMessaging>("Select mess.* from TCareMessagings mess " +
                        "Left join TCareCampaigns camp on camp.Id = mess.TCareCampaignId " +
                        "Where Date < GETDATE() And camp.Active = 1 ");

                    foreach (var messaging in messagings)
                    {
                        
                        //get all message in messaging check state = 'waiting' send message 
                        var messages = await conn.QueryAsync<TCareMessage>("Select * from TCareMessages " +
                            "where TCareMessagingId = @id And State = 'waiting' ", new { id = messaging.Id });

                        //Gửi tin nhắn 
                        var tasks = messages.Select(x => SendMessgeToPage(db, x)).ToList();
                        var limit = 200;
                        var offset = 0;
                        var subTasks = tasks.Skip(offset).Take(limit).ToList();
                        while (subTasks.Any())
                        {
                            await Task.WhenAll(subTasks);
                            offset += limit;
                            subTasks = tasks.Skip(offset).Take(limit).ToList();
                        }

                        UpdateStateMessaging(conn, messaging.Id);
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
        }

        public async Task TCareTakeMessage(string db)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.CatalogConnection);
            if (db != "localhost")
                builder["Database"] = $"TMTDentalCatalogDb__{db}";

            //FacebookPage channel = null;
            //string channelType = "";
            //FacebookUserProfile facebookUserProfile = null;
            //List<TCareCampaign> listCampaigns = new List<TCareCampaign>();
            //Partner partner = null;
            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    conn.Open();

                 
                        //tìm danh sách các chiến dịch đang active
                        var activeCampaigns = await conn.QueryAsync<TCareCampaign>("SELECT * FROM TCareCampaigns" +
                            " where Active = 1 ");

                    foreach (var campaign in activeCampaigns)
                    {
                        //với mỗi chiến dịch lọc danh sách khách hàng
                        var partner_ids = await SearchPartnerIdsV2(campaign.GraphXml, conn);
                        if (partner_ids == null || !partner_ids.Any())
                            continue;

                            var content = GetCampaignContent(campaign.GraphXml);
                            if (string.IsNullOrEmpty(content))
                                continue;

                        //tạo tin nhắn gửi hàng loạt đưa vào hàng đợi
                        var scheduleDate = GetScheduleDateCampaign(campaign);
                        var messaging = new TCareMessaging()
                        {
                            Id = GuidComb.GenerateComb(),
                            ScheduleDate = scheduleDate,
                            State = "in_queue",
                            Content = content,
                            TCareCampaignId = campaign.Id,
                            FacebookPageId = campaign.FacebookPageId
                        };

                            await CreateMessaging(conn, messaging);

                        //Gửi tin nhắn hàng loạt
                        await SendMessaging(conn, messaging, partner_ids);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
        }

        private async Task SendMessaging(SqlConnection conn, TCareMessaging messaging, IEnumerable<Guid> partnerIds)
        {
            //Tìm profiles sẽ gửi cho list partnerIds
            var profiles = conn.Query<FacebookUserProfile>("select * from FacebookUserProfiles p " +
               "where p.PartnerId in @partnerIds and p.FbPageId = @pageId ", new { partnerIds = partnerIds, pageId = messaging.FacebookPageId }).ToList();

            var profileDict = profiles.ToDictionary(x => x.PartnerId, x => x);

            var partners = conn.Query<PartnerDataPersonalize>("select p.Id, p.Name, p0.Name from Partners p " +
                    "left join PartnerTitles p0 on p.TitleId = p0.Id " +
                    "where p.Id in @ids", new { ids = partnerIds }).ToList();

            var partnerDict = partners.ToDictionary(x => x.Id, x => x);

            var channel = conn.Query<FacebookPage>("select Id, PageName from FacebookPages where Id = @id", new { id = messaging.FacebookPageId }).FirstOrDefault();

            var messages = new List<TCareMessage>();

            foreach (var partnerId in partnerIds)
            {
                if (!profileDict.ContainsKey(partnerId))
                    continue;

                var profile = profileDict[partnerId];

                if (!partnerDict.ContainsKey(partnerId))
                    continue;

                var partner = partnerDict[partnerId];

                var messageContent = PersonalizedContent(partner, channel, messaging.Content);

                var message = new TCareMessage()
                {
                    Id = GuidComb.GenerateComb(),
                    ProfilePartnerId = profile.Id,
                    ChannelSocicalId = messaging.FacebookPageId,
                    CampaignId = messaging.TCareCampaignId,
                    PartnerId = partnerId,
                    MessageContent = messageContent,
                    TCareMessagingId = messaging.Id,
                    State = "waiting",
                    ScheduledDate = messaging.ScheduleDate
                };

                messages.Add(message);
            }

            await conn.ExecuteAsync("insert into TCareMessages " +
                    "(Id,ProfilePartnerId,ChannelSocicalId,CampaignId,PartnerId,MessageContent,TCareMessagingId,State,ScheduledDate) " +
                    "values (@Id,@ProfilePartnerId,@ChannelSocicalId,@CampaignId,@PartnerId,@MessageContent,@TCareMessagingId,@State,@ScheduledDate)", messages.ToArray());
        }

        private DateTime GetScheduleDateCampaign(TCareCampaign campaign)
        {
            //xác định thời điểm gửi tin của chiến dịch
            var date = DateTime.Now;
            if (campaign.SheduleStart.HasValue)
                date = date.AddHours(campaign.SheduleStart.Value.Hour).AddMinutes(campaign.SheduleStart.Value.Minute);
            return date;
        }

        private string GetCampaignContent(string graphXml)
        {
            if (string.IsNullOrEmpty(graphXml))
                return string.Empty;
            XmlSerializer serializer = new XmlSerializer(typeof(MxGraphModel));
            MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(graphXml));
            MxGraphModel resultingMessage = (MxGraphModel)serializer.Deserialize(memStream);
            var sequence = resultingMessage.Root != null ? resultingMessage.Root.Sequence : null;
            if (sequence != null)
                return sequence.Content;
            return string.Empty;
        }

        public async Task CreateMessaging(SqlConnection conn, TCareMessaging messaging)
        {
            await conn.ExecuteAsync("insert into TCareMessagings " +
                 "(Id,Content,TCareCampaignId, Date,State,FacebookPageId) " +
                 "Values (@Id, @Content, @TCareCampaignId, @Date,@State,@FacebookPageId)",
                 new
                 {
                     Id = messaging.Id,
                     Content = messaging.Content,
                     TCareCampaignId = messaging.TCareCampaignId,
                     Date = messaging.ScheduleDate,
                     State = "in_queue",
                     FacebookPageId = messaging.FacebookPageId
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

        public void UpdateErrorMessage(SqlConnection conn, Guid id)
        {
            conn.Execute("UPDATE TCareMessages SET State = 'exception', TCareMessagingTraceId = @TCareMessagingTraceId WHERE Id = @Id ",
                new
                {
                    Id = id,
                });
        }

        public void UpdateStateMessaging(SqlConnection conn, Guid id)
        {
            conn.Execute("UPDATE TCareMessagings SET State = 'done' WHERE Id = @Id ",
                new
                {
                    Id = id,
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
                    check = await SendMessagePageFacebook(profile, mess.MessageContent, db, channelSocial, conn, trace_Id, mess.Id);
                    if (check)
                    {
                        if (campaign.TagId.HasValue)
                            SetTagForPartner(profile.PartnerId.Value, campaign.TagId.Value, conn);

                        UpdateMessage(conn, mess.Id, trace_Id);
                    }
                    break;
                case "zalo":
                    check = await SendMessagePageZalo(profile, mess.MessageContent, db, channelSocial, conn, trace_Id, mess.Id);
                    if (check)
                    {
                        if (campaign.TagId.HasValue)
                        SetTagForPartner(profile.PartnerId.Value, campaign.TagId.Value, conn);

                        UpdateMessage(conn, mess.Id, trace_Id);
                    }
                    break;
                default:
                    break;
            }
        }

        private async Task<bool> SendMessagePageZalo(FacebookUserProfile profile, string messageContent, string db, FacebookPage channelSocial, SqlConnection conn, Guid trace_Id, Guid mess_Id)
        {
            var zaloClient = new ZaloClient(channelSocial.PageAccesstoken);
            var sendResult = zaloClient.sendTextMessageToUserId(profile.PSID, messageContent).ToObject<SendMessageZaloResponse>();
            if (sendResult.error != 0)
            {
                await conn.ExecuteAsync("update TCareMessingTraces set Exception=@exception, Error=@error where Id=@id", new { exception = DateTime.Now, error = sendResult.error, id = trace_Id });
                UpdateErrorMessage(conn, mess_Id);
                return false;
            }
            else
            {
                await conn.ExecuteAsync("update TCareMessingTraces set MessageId=@messageId, Sent=@sent where Id=@id", new { messageId = sendResult.data.message_id, sent = DateTime.Now, id = trace_Id });
                return true;
            }
        }

        private async Task<bool> SendMessagePageFacebook(FacebookUserProfile profile, string messageContent, string db, FacebookPage channelSocial, SqlConnection conn, Guid trace_Id, Guid mess_Id)
        {
            var sendResult = await _fbMessageSender.SendMessageTCareTextAsync(messageContent, profile.PSID, channelSocial.PageAccesstoken);
            if (!string.IsNullOrEmpty(sendResult.error))
            {
                await conn.ExecuteAsync("update TCareMessingTraces set Exception=@exception, Error=@error where Id=@id", new { exception = DateTime.Now, error = sendResult.error, id = trace_Id });
                UpdateErrorMessage(conn, mess_Id);
                return false;
            }
            else
            {
                await conn.ExecuteAsync("update TCareMessingTraces set MessageId=@messageId where Id=@id", new { messageId = sendResult.message_id, id = trace_Id });
                BackgroundJob.Enqueue(() => GetInfoMessageFBApi(db, trace_Id, channelSocial.PageAccesstoken, sendResult.message_id));
                return true;
            }
        }

        private string PersonalizedContent(PartnerDataPersonalize partner, FacebookPage channelSocial, string content)
        {
            var messageContent = content.Replace("@ten_khach_hang", partner.Name.Split(' ').Last())
                .Replace("@fullname_khach_hang", partner.Name)
                .Replace("@ten_page", channelSocial.PageName)
                .Replace("@danh_xung_khach_hang", partner.Title ?? "");
            return messageContent;
        }

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
            if (string.IsNullOrEmpty(graphXml))
                return null;

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

            //nếu không có condition nào thì gửi tất cả khách hàng
            if (conditions.Any())
            {
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
            else
            {
                return conn.Query<Guid>("select p.Id " +
                        "from Partners p " +
                        $"where p.Customer = 1").ToList();
            }
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

    public class LastUsedServiceDate
    {
        public string Type { get; set; }

        public int NumberInterval { get; set; }

        public string Interval { get; set; }

        public Guid? ProductId { get; set; }
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

    public class PartnerDataPersonalize
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Danh xưng
        /// </summary>
        public string Title { get; set; }
    }

}
