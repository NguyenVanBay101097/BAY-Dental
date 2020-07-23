using ApplicationCore.Entities;
using Dapper;
using Facebook.ApiClient.ApiEngine;
using Facebook.ApiClient.Constants;
using Facebook.ApiClient.Interfaces;
using Hangfire;
using Infrastructure.Data;
using Microsoft.Extensions.Options;
using MyERP.Utilities;
using Newtonsoft.Json;
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
        private readonly IFacebookMessageSender _fbMessageSender;
        public TCareJobService(IOptions<ConnectionStrings> connectionStrings, IFacebookMessageSender fbMessageSender)
        {

            _connectionStrings = connectionStrings?.Value;
            _fbMessageSender = fbMessageSender;
        }

        public void Run(string db, Guid campaignId)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.CatalogConnection);
            if (db != "localhost")
                builder["Database"] = $"TMTDentalCatalogDb__{db}";
            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    conn.Open();
                    //var date = DateTime.UtcNow;
                    var campaign = conn.Query<TCareCampaign>("SELECT * FROM TCareCampaigns WHERE Id = @id", new { id = campaignId }).FirstOrDefault();

                    XmlSerializer serializer = new XmlSerializer(typeof(MxGraphModel));
                    MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(campaign.GraphXml));
                    MxGraphModel CampaignXML = (MxGraphModel)serializer.Deserialize(memStream);

                    //if (CampaignXML.Root.Sequence.MethodType == "interval")
                    //{
                    //    var intervalNumber = int.Parse(CampaignXML.Root.Sequence.IntervalNumber);
                    //    if (CampaignXML.Root.Sequence.IntervalType == "hours")
                    //        date = date.AddHours(intervalNumber);
                    //    else if (CampaignXML.Root.Sequence.IntervalType == "minutes")
                    //        date = date.AddMinutes(intervalNumber);
                    //    else if (CampaignXML.Root.Sequence.IntervalType == "days")
                    //        date = date.AddDays(intervalNumber);
                    //    else if (CampaignXML.Root.Sequence.IntervalType == "months")
                    //        date = date.AddMonths(intervalNumber);
                    //    else if (CampaignXML.Root.Sequence.IntervalType == "weeks")
                    //        date = date.AddDays((intervalNumber) * 7);
                    //}
                    //else
                    //{
                    //    if (!string.IsNullOrEmpty(CampaignXML.Root.Sequence.SheduleDate))
                    //        date = DateTime.Parse(CampaignXML.Root.Sequence.SheduleDate).ToUniversalTime();
                    //}

                    var partner_ids = SearchPartnerIds(CampaignXML.Root.Rule.Condition, CampaignXML.Root.Rule.Logic, conn);
                    foreach(var partner_id in partner_ids)
                        BackgroundJob.Enqueue(() => SendMessageSocial(campaignId, db, partner_id));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }



       
        // Xử lý nhiều rules
        //public IEnumerable<Guid> SearchPartnerIdsInRules(IEnumerable<Rule> rules, string typeRule, SqlConnection conn)
        //{
        //    var partnerIds = new List<Guid>();
        //    foreach (var rule in rules)
        //        partnerIds.AddRange(SearchPartnerIds(rule.Condition, typeRule, conn));

        //    partnerIds = partnerIds.Distinct().ToList();

        //    return partnerIds;
        //}    

        public IEnumerable<Guid> SearchPartnerIds(IEnumerable<Condition> conditions, string typeRule, SqlConnection conn)
        {
          
            var lstRule = new List<RulePartnerIds>();
            var partner_ids = new List<Guid>();
            var builder = new SqlBuilder();
            var lst = new Dictionary<string, string>();
            lst.Add("contains", "=");
            lst.Add("not_contains", "!=");
            //lst.Add("contains", "");
            //lst.Add("not_contains", "");
            lst.Add("lte", "<=");
            lst.Add("gte", ">=");
            var PartnerIds = conn.Query<Guid>("" +
                                      "Select pn.Id " +
                                      "From Partners pn " +
                                      "Where pn.Customer = 1 "
                                      ).ToList();

            if (conditions.Count() > 0)
            {
                foreach (var condition in conditions)
                {
                    switch (condition.Type)
                    {

                        case "birthday":
                            // lấy ra danh sách khách dựa vào sinh nhật của khách hàng
                            var today = DateTime.Today;
                            var date = today.AddDays(int.Parse(condition.Value));
                            var birthdayPartnerIds = conn.Query<Guid>("" +
                                           "Select pn.Id " +
                                           "From Partners pn " +
                                           "Where pn.Customer = 1 and pn.BirthDay = @day AND pn.BirthMonth = @month ", new { day = date.Day, month = date.Month }
                                           ).ToList();
                            lstRule.Add(new RulePartnerIds() { Ids = birthdayPartnerIds });
                            break;
                        case "lastSaleOrder":
                            // lấy ra danh sách khách dựa vào phiểu điều trị cuối lấy ra ngày điều trị cuối của khách hàng
                            var lastSaleOrderPartnerIds = conn.Query<Guid>("" +
                                    "Select pn.Id From Partners pn " +
                                    "Left join SaleOrders sale ON sale.PartnerId = pn.Id " +
                                    "Where pn.Customer = 1 and sale.State = @sale " +
                                    "Group by pn.Id " +
                                    "Having (Max(sale.DateOrder) < DATEADD(day, -@number, GETDATE())) ", new { number = int.Parse(condition.Value), sale = "('sale','done')" }).ToList();
                            lstRule.Add(new RulePartnerIds() { Ids = lastSaleOrderPartnerIds });
                            break;
                        case "categPartner":
                            // lấy ra danh sách khách dựa vào nhóm khách hàng
                            foreach (var kvp in lst.Where(x => x.Key == condition.Op))
                            {
                                switch (kvp.Key)
                                {
                                    case "contains":
                                        //danh sách khách hàng thuộc nhóm khách hàng
                                        var categPartnerIds = conn.Query<Guid>("" +
                                                       "Select pn.Id " +
                                                       "From Partners pn " +
                                                       "Left join PartnerPartnerCategoryRel rel On rel.PartnerId = pn.Id " +
                                                       "Left join PartnerCategories cpt On cpt.id = rel.CategoryId " +
                                                       $"Where pn.Customer = 1 and cpt.Id {kvp.Value} @cateId ", new { cateId = condition.Value }).ToList();
                                        lstRule.Add(new RulePartnerIds() { Ids = categPartnerIds });
                                        break;
                                    case "not_contains":
                                        //danh sách khách hàng không thuộc nhóm khách hàng
                                        var categNotPartnerIds = conn.Query<Guid>("" +
                                                       "Select pn.Id " +
                                                       "From Partners pn " +
                                                       "Left join PartnerPartnerCategoryRel rel On rel.PartnerId = pn.Id " +
                                                       "Left join PartnerCategories cpt On cpt.id = rel.CategoryId " +
                                                       $"Where pn.Customer = 1 and cpt.Id {kvp.Value} @cateId ", new { cateId = condition.Value }).ToList();
                                        lstRule.Add(new RulePartnerIds() { Ids = categNotPartnerIds });
                                        break;
                                    default:
                                        throw new NotSupportedException(string.Format("Not support Operator {0}!", condition.Value));
                                }
                            }

                            break;
                        case "usedService":
                            // lấy ra danh sách khách dựa vào dịch vụ khách hàng đã sử dụng
                            foreach (var kvp in lst.Where(x => x.Key == condition.Op))
                            {
                                switch (kvp.Key)
                                {
                                    case "contains":
                                        //danh sách khách hàng đã sử dụng dịch vụ
                                        var usedServicePartnerIds = conn.Query<Guid>("" +
                                        "Select pn.Id " +
                                        "From Partners pn " +
                                        "Where pn.Customer = 1 and EXISTS (Select orlines.OrderPartnerId " +
                                        "From SaleOrderLines orlines " +
                                        "Left join Products sp On sp.Id = orlines.ProductId " +
                                        $"Where sp.id = @serviceId And sp.Type2 = 'service' AND orlines.State in ('sale','done') AND orlines.OrderPartnerId = pn.Id Group by orlines.OrderPartnerId) ", new { serviceId = condition.Value })
                                            .ToList();
                                        lstRule.Add(new RulePartnerIds() { Ids = usedServicePartnerIds });
                                        break;
                                    case "not_contains":
                                        //danh sách khách hàng chưa sử dụng dịch vụ 
                                        var usedServiceNotPartnerIds = conn.Query<Guid>("" +
                                       "Select pn.Id " +
                                       "From Partners pn " +
                                       "Where pn.Customer = 1 and NOT EXISTS (Select orlines.OrderPartnerId From SaleOrderLines orlines " +
                                       "Left join Products sp On sp.Id = orlines.ProductId " +
                                      $"Where sp.id = @serviceId And sp.Type2 = 'service' AND orlines.State in ('sale','done') AND orlines.OrderPartnerId = pn.Id Group by orlines.OrderPartnerId) ", new { serviceId = condition.Value })
                                           .ToList();
                                        lstRule.Add(new RulePartnerIds() { Ids = usedServiceNotPartnerIds });
                                        break;
                                    default:
                                        throw new NotSupportedException(string.Format("Not support Operator {0}!", condition.Value));
                                }
                            }
                            break;
                        case "usedCategService":
                            //lấy ra danh sách khách dựa vào nhóm dịch vụ khách hàng đã sử dụng
                            foreach (var kvp in lst.Where(x => x.Key == condition.Op))
                            {
                                switch (kvp.Key)
                                {
                                    case "contains":
                                        //danh sách khách hàng đã sử dụng nhóm dịch vụ
                                        var usedCategServicePartnerIds = conn.Query<Guid>("" + "Select pn.Id " +
                                            "From Partners pn " +
                                            "Where pn.Customer = 1 and EXISTS(Select orlines.OrderPartnerId " +
                                            "From SaleOrderLines orlines " +
                                            "Left join Products sp On sp.Id = orlines.ProductId " +
                                            "Left join ProductCategories csp On csp.Id = sp.CategId " +
                                            "Where csp.Id = @categService And sp.Type2 = 'service' AND orlines.State in ('sale','done') AND orlines.OrderPartnerId = pn.Id Group by orlines.OrderPartnerId) ", new { categService = condition.Value })
                                            .ToList();

                                        lstRule.Add(new RulePartnerIds() { Ids = usedCategServicePartnerIds });
                                        break;
                                    case "not_contains":
                                        //danh sách khách hàng chưa sử dụng nhóm dịch vụ
                                        var usedCategServiceNotPartnerIds = conn.Query<Guid>("" + "Select pn.Id " +
                                            "From Partners pn " +
                                           "Where pn.Customer = 1 and NOT EXISTS(Select orlines.OrderPartnerId " +
                                           "From SaleOrderLines orlines " +
                                           "Left join Products sp On sp.Id = orlines.ProductId " +
                                           "Left join ProductCategories csp On csp.Id = sp.CategId " +
                                           "Where csp.Id = @categService And sp.Type2 = 'service' AND orlines.State in ('sale','done') AND orlines.OrderPartnerId = pn.Id Group by orlines.OrderPartnerId) ", new { categService = condition.Value })
                                            .ToList();
                                        lstRule.Add(new RulePartnerIds() { Ids = usedCategServiceNotPartnerIds });
                                        break;
                                    default:
                                        throw new NotSupportedException(string.Format("Not support Operator {0}!", condition.Value));
                                }
                            }

                            break;
                        case "lastExamination":
                            //ngày khám cuối cùng sau bao nhiêu ngày 
                             var lastExaminationPartnerIds = conn.Query<Guid>("" +
                                    "Select pn.Id From Partners pn " +
                                    "Left join SaleOrders sale ON sale.PartnerId = pn.Id " +
                                    "Left join DotKhams dk ON dk.SaleOrderId = sale.Id " +
                                    "Where pn.Customer = 1 and sale.State in ('sale','done') " +
                                    "Group by pn.Id " +
                                    "Having (Max(dk.Date) <= DATEADD(day, -@number, GETDATE())) ", new { number = int.Parse(condition.Value) }).ToList();
                             lstRule.Add(new RulePartnerIds() { Ids = lastExaminationPartnerIds });
                             break;
                        case "lastAppointment":
                            //lịch hẹn tiếp theo/gần đây trước bao nhiêu ngày
                            var lastAppointmentPartnerIds = conn.Query<Guid>("" +
                                    "Select pn.Id From Partners pn " +
                                    "Left join Appointments am ON am.PartnerId = pn.Id " +                                
                                    "Where pn.Customer = 1 " +
                                    "Group by pn.Id " +
                                    "Having (Max(am.Date) >= DATEADD(day, @number, GETDATE())) ", new { number = int.Parse(condition.Value) }).ToList();
                            lstRule.Add(new RulePartnerIds() { Ids = lastAppointmentPartnerIds });
                            break;
                    }

                }

                if (typeRule == "and")
                {
                    foreach (var rule in lstRule)
                        partner_ids = PartnerIds.Intersect(rule.Ids).Distinct().ToList();

                }
                else if (typeRule == "or")
                {
                    foreach (var rule in lstRule)
                        partner_ids = PartnerIds.Union(rule.Ids).Distinct().ToList();
                }


                partner_ids = partner_ids.Distinct().ToList();
            }
            else
            {
                partner_ids = PartnerIds;
            }
                


                //var partner_ids = conn.Query<Guid>(sqltemplate.RawSql, sqltemplate.Parameters).Distinct().ToList();
            return partner_ids;
        }

        public async Task SendMessageSocial(Guid? campaignId = null,
            string db = null, Guid? partner_id = null)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.CatalogConnection);
            if (db != "localhost")
                builder["Database"] = $"TMTDentalCatalogDb__{db}";

            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                conn.Open();

                var partner_ids = new List<Guid>().AsEnumerable();
                var campaign = conn.Query<TCareCampaign>("SELECT * FROM TCareCampaigns WHERE Id = @id", new { id = campaignId }).FirstOrDefault();
                if (campaign == null)
                    return;

                XmlSerializer serializer = new XmlSerializer(typeof(MxGraphModel));
                MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(campaign.GraphXml));
                MxGraphModel resultingMessage = (MxGraphModel)serializer.Deserialize(memStream);

                var sequence = resultingMessage.Root.Sequence;
                var rule = resultingMessage.Root.Rule;
                if (sequence == null || rule == null)
                    return;

                if (string.IsNullOrEmpty(sequence.Content))
                    return;

                var channelSocial = conn.Query<FacebookPage>("" +
                     "SELECT * " +
                     "FROM FacebookPages " +
                     "where Id = @id" +
                     "", new { id = sequence.ChannelSocialId }).FirstOrDefault();

                if (channelSocial == null)
                    return;

                var profiles = GetUserProfiles(Guid.Parse(sequence.ChannelSocialId), partner_id.Value, conn);
                if (!profiles.Any())
                    return;

                var profile = profiles.First();

                //Xử lý cá nhân hóa nội dung gửi tin
                var messageContent = sequence.Content;

                //Xử lý gửi tin
                if (sequence.ChannelType == "priority")
                {

                }
                else if (sequence.ChannelType == "fixed")
                {
                    var now = DateTime.Now;
                    var track_id = GuidComb.GenerateComb();
                    var channelSocialId = Guid.Parse(sequence.ChannelSocialId);

                    await conn.ExecuteAsync("insert into TCareMessingTraces(Id,TCareCampaignId,PSID,PartnerId,Type,ChannelSocialId,DateCreated,LastUpdated) Values (@Id,@TCareCampaignId,@PSID,@PartnerId,@Type,@ChannelSocialId,@DateCreated,@LastUpdated)", new { Id = track_id, TCareCampaignId = campaignId, PSID = profile.PSID, PartnerId = profile.PartnerId, Type = channelSocial.Type, ChannelSocialId = channelSocialId, DateCreated = now, LastUpdated = now });

                    if (channelSocial.Type == "facebook")
                    {
                        var sendResult = await _fbMessageSender.SendMessageTCareTextAsync(messageContent, profile.PSID, channelSocial.PageAccesstoken);
                        if (!string.IsNullOrEmpty(sendResult.error))
                        {
                            await conn.ExecuteAsync("update TCareMessingTraces set Exception=@exception, Error=@error where Id=@id", new { exception = DateTime.Now, error = sendResult.error, id = track_id });
                        }
                        else
                        {
                            await conn.ExecuteAsync("update TCareMessingTraces set MessageId=@messageId where Id=@id", new { messageId = sendResult.message_id, id = track_id });
                            BackgroundJob.Enqueue(() => GetInfoMessageFBApi(db, track_id, channelSocial.PageAccesstoken, sendResult.message_id));
                        }
                    }
                    else if (channelSocial.Type == "zalo")
                    {
                        var sent = DateTime.Now;
                        var zaloClient = new ZaloClient(channelSocial.PageAccesstoken);
                        var sendResult = zaloClient.sendTextMessageToUserId(profile.PSID, messageContent).ToObject<SendMessageZaloResponse>();

                        if (sendResult.error != 0)
                            await conn.ExecuteAsync("update TCareMessingTraces set Exception=@exception, Error=@error where Id=@id", new { exception = DateTime.Now, error = sendResult.error, id = track_id });
                        else
                            await conn.ExecuteAsync("update TCareMessingTraces set MessageId=@messageId, Sent=@sent where Id=@id", new { messageId = sendResult.data.message_id, sent = sent, id = track_id });
                    }
                }
            }
        }

        private static IEnumerable<FacebookUserProfile> GetUserProfiles(Guid ChannelSocialId, Guid partner_id,
           SqlConnection conn = null)
        {
            var profiles = conn.Query<FacebookUserProfile>("select * from FacebookUserProfiles where FbPageId = @PageId and PartnerId = @PartnerId",
                new { PageId = ChannelSocialId, PartnerId = partner_id }).ToList();

            return profiles;
        }

        private static IEnumerable<FacebookUserProfile> GetUserProfiles(Guid ChannelSocialId, IEnumerable<Guid> partIds,
           SqlConnection conn = null)
        {     
            var builder = new SqlBuilder();
            var sqltemplate = builder.AddTemplate("SELECT /**select**/ FROM FacebookUserProfiles us /**leftjoin**/ /**where**/ /**orderby**/ ");

            builder.Select("us.* ");
            builder.Where("us.FbPageId = @PageId ", new { PageId = ChannelSocialId });
            var iUserprofiles = conn.Query<FacebookUserProfile>(sqltemplate.RawSql, sqltemplate.Parameters).ToList();


            // lấy danh sách userprofiles theo filter
            //var profiles = GetProfilesActivity(activity, pageId, conn);
            var lstUserProfile = iUserprofiles.Where(x => partIds.Any(s => s == x.PartnerId)).ToList();

            return lstUserProfile;

        }

       



        //public async Task SendMessageAndTrace(SqlConnection conn, string text, FacebookUserProfile profile, string access_token, Guid campaignId , Guid channelSocialId)
        //{
        //    //var now = DateTime.Now;
        //    //var date = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
        //    //date = date.AddSeconds(-1);

        //    ////khách hàng
        //    //var partner = conn.Query<Partner>("" +
        //    //             "SELECT * " +
        //    //             "FROM Partners " +
        //    //             "where Id = @id" +
        //    //             "", new { id = profile.PartnerId }).FirstOrDefault();

        //    //if (partner == null)
        //    //    return;

        //    /////chi nhánh
        //    //var company = conn.Query<Company>("" +
        //    //            "SELECT * " +
        //    //            "FROM Companies " +
        //    //            "where Id = @id" +
        //    //            "", new { id = partner.CompanyId }).FirstOrDefault();

        //    //if (company == null)
        //    //    return;

        //    //var sendResult = await _fbMessageSender.SendMessageTCareTextAsync(text.Replace("{{ten_khach_hang}}", partner.Name).Replace("{{gioi_tinh}}", partner.Gender == "male" ? "anh" : "chị").Replace("{{ten_chi_nhanh}}", company.Name), profile.PSID, access_token);
        //    //var sendResult = await _fbMessageSender.SendMessageTCareTextAsync(text, profile.PSID, access_token);
        //    //if (sendResult == null)
        //    //    await conn.ExecuteAsync("insert into TCareMessingTraces(Id,Exception,TCareCampaignId,PSID,PartnerId,Type,ChannelSocialId) Values (@Id,@Exception,@TCareCampaignId,@PSID,@PartnerId,@Type,@ChannelSocialId)", new { Id = GuidComb.GenerateComb(), Exception = date, TCareCampaignId = campaignId, PSID = profile.PSID, PartnerId = profile.PartnerId, Type = "facebook" , ChannelSocialId = channelSocialId });
        //    //else
        //    //    await conn.ExecuteAsync("insert into TCareMessingTraces(Id,Sent,TCareCampaignId,MessageId,PSID,PartnerId,Type,ChannelSocialId) Values (@Id,@Sent,@TCareCampaignId,@MessageId,@PSID,@PartnerId,@Type,@ChannelSocialId)", new { Id = GuidComb.GenerateComb(), Sent = date, TCareCampaignId = campaignId, MessageId = sendResult.message_id, PSID = profile.PSID, PartnerId = profile.PartnerId, Type = "facebook", ChannelSocialId = channelSocialId });
        //}

        public async Task SendMessageAndTraceZalo(SqlConnection conn, string text, FacebookUserProfile profile, string access_token, Guid campaignId, Guid channelSocialId)
        {
            var now = DateTime.Now;
            var date = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            date = date.AddSeconds(-1);
            //khách hàng
            var partner = conn.Query<Partner>("" +
                         "SELECT * " +
                         "FROM Partners " +
                         "where Id = @id" +
                         "", new { id = profile.PartnerId }).FirstOrDefault();

            if (partner == null)
                return;

            ///chi nhánh
            var company = conn.Query<Company>("" +
                        "SELECT * " +
                        "FROM Companies " +
                        "where Id = @id" +
                        "", new { id = partner.CompanyId }).FirstOrDefault();

            if (company == null)
                return;

            var zaloClient = new ZaloClient(access_token);
            var sendResult = zaloClient.sendTextMessageToUserId(profile.PSID, text.Replace("{{ten_khach_hang}}", partner.Name).Replace("{{gioi_tinh}}", partner.Gender == "male" ? "anh" : "chị").Replace("{{ten_chi_nhanh}}", company.Name)).Root.ToObject<SendMessageZaloResponse>().data;
            if (sendResult == null)
                await conn.ExecuteAsync("insert into TCareMessingTraces(Id,Exception,TCareCampaignId,PSID,PartnerId,Type,ChannelSocialId) Values (@Id,@Exception,@TCareCampaignId,@PSID,@PartnerId,@Type,@ChannelSocialId)", new { Id = GuidComb.GenerateComb(), Exception = date, TCareCampaignId = campaignId, PSID = profile.PSID, PartnerId = profile.PartnerId, Type = "zalo", ChannelSocialId = channelSocialId });
            else
                await conn.ExecuteAsync("insert into TCareMessingTraces(Id,Sent,TCareCampaignId,MessageId,PSID,PartnerId,Type,ChannelSocialId) Values (@Id,@Sent,@TCareCampaignId,@MessageId,@PSID,@PartnerId,@Type,@ChannelSocialId)", new { Id = GuidComb.GenerateComb(), Sent = date, TCareCampaignId = campaignId, MessageId = sendResult.message_id, PSID = profile.PSID, PartnerId = profile.PartnerId, Type = "zalo", ChannelSocialId = channelSocialId });


        }

        [AutomaticRetry]

        public async Task GetInfoMessageFBApi(string db, Guid track_id, string access_token, string message_id)
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
                    await conn.ExecuteAsync("update TCareMessingTraces set Sent=@sent where Id=@id", new { Id = track_id, Sent = result.created_time.ToLocalTime() });
                }
            }
        }
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

}
