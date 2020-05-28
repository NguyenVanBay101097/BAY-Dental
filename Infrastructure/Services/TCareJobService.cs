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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
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
                       
                        var messaging = conn.Query<TCareMessaging>("SELECT * FROM TCareMessagings WHERE TCareCampaignId = @id", new { id = campaign.Id }).FirstOrDefault();
                        var campaignXml = ConvertXmlCampaign(campaign.Xml);
                        if (messaging.MethodType == "interval")
                        {
                            var intervalNumber = messaging.IntervalNumber ?? 0;
                            if (messaging.IntervalType == "hours")
                                date = date.AddHours(intervalNumber);
                            else if (messaging.IntervalType == "minutes")
                                date = date.AddMinutes(intervalNumber);
                            else if (messaging.IntervalType == "days")
                                date = date.AddDays(intervalNumber);
                            else if (messaging.IntervalType == "months")
                                date = date.AddMonths(intervalNumber);
                            else if (messaging.IntervalType == "weeks")
                                date = date.AddDays((intervalNumber) * 7);
                            
                            var jobId = BackgroundJob.Schedule(() => SendMessageSocial(messaging.TCareCampaignId, messaging.Content,db), date);
                            if (string.IsNullOrEmpty(jobId))
                                throw new Exception("Can't not schedule job");
                        }
                        else
                        {
                            if (!messaging.SheduleDate.HasValue)
                                throw new Exception("Không tìm thấy thời gian cố định . Vui lòng kiểm tra lại !!!");
                            date = messaging.SheduleDate.Value;
                            var jobId = BackgroundJob.Schedule(() => SendMessageSocial(messaging.TCareCampaignId, messaging.Content, db), date);
                            if (string.IsNullOrEmpty(jobId))
                                throw new Exception("Can't not schedule job");
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

        public TCareCampaign ConvertXmlCampaign(string xmlFilePath)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xmlFilePath);
            var text = doc;

            string contents = File.ReadAllText(xmlFilePath);          
            XmlSerializer serializer = new XmlSerializer(typeof(TCareCampaign));

            StreamReader reader = new StreamReader(contents);
            var campaignXml = (TCareCampaign)serializer.Deserialize(reader);
            reader.Close();

            return campaignXml;
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
                var date = today.AddDays(beforeDays);

                var partner_ids = conn.Query<Guid>("SELECT Id FROM Partners WHERE Customer = 1 AND BirthDay = @day AND BirthMonth = @month", new { day = date.Day, month = date.Month }).ToList();
                return partner_ids;
            }

            return new List<Guid>();
        }

        public async Task SendMessageSocial(Guid? campaignId = null, string content = null,
            string db = null)
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

                    var lstpartnerId = new List<Guid>();
                    var profiles = new List<FacebookUserProfile>().AsEnumerable();
                    //Get list rules in campaign
                    var rules = conn.Query<TCareRule>("SELECT * FROM TCareRules WHERE CampaignId = @id", new { id = campaignId }).ToList();

                    //Get partnerIds in list rules
                    var partner_ids = SearchPartnerRules(rules, conn);
                    //if (partner_ids.Count() == 0)
                    //    continue;

                    var messaging = conn.Query<TCareMessaging>("SELECT * FROM TCareMessagings WHERE TCareCampaignId = @id", new { id = campaignId }).FirstOrDefault();
                    if (messaging == null)
                        return;

                    var channelSocial = conn.Query<FacebookPage>("" +
                         "SELECT * " +
                         "FROM FacebookPages " +
                         "where Id = @id" +
                         "", new { id = messaging.ChannelSocialId }).FirstOrDefault();

                    if (channelSocial == null)
                        return;

                    profiles = GetUserProfiles(messaging.ChannelSocialId.Value, partner_ids, conn);
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
                    partner_ids = partner_ids.Where(x => !profiles.Any(s => s.PartnerId == x)).ToList();

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
                            if (partner_ids.Count() == 0)
                                break;
                            profiles = GetUserProfiles(channel.Id, partner_ids, conn);
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
                            partner_ids = partner_ids.Where(x => !profiles.Any(s => s.PartnerId == x)).ToList();
                        }



                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
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
            var lstUserProfile = iUserprofiles.Where(x => partIds.Any(s=> s == x.PartnerId )).ToList();

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

    

    //[Serializable()]
    //public class TCareCampaignXml
    //{
    //    [XmlArray("TCareRules")]
    //    [XmlArrayItem("TCareRule", typeof(TCareRule))]
    //    public TCareRule TCareRule { get; set; }

    //    [XmlElement("TCareMessaging")]
    //    public TCareMessaging TCareMessaging { get; set; }

    //}

    //public class TCareRule
    //{

    //    public string Type { get; set; }

    //    [XmlArray("Properties")]
    //    [XmlArrayItem("TCareProperty", typeof(TCareProperty))]
    //    public TCareProperty TCareProperty { get; set; }
    //}

    //public class TCareProperty {

    //    [XmlElement("Name")]
    //    public string Name { get; set; }

    //    [XmlElement("Type")]
    //    /// <summary>
    //    /// Text, Integer, DateTime, Decimal, Double, Many2One
    //    /// </summary>
    //    public string Type { get; set; }

    //    [XmlElement("ValueText")]
    //    public string ValueText { get; set; }

    //    [XmlElement("ValueInteger")]
    //    public int? ValueInteger { get; set; }

    //    [XmlElement("ValueDateTime")]
    //    public DateTime? ValueDateTime { get; set; }

    //    [XmlElement("ValueDecimal")]
    //    public decimal? ValueDecimal { get; set; }

    //    [XmlElement("ValueDouble")]
    //    public double? ValueDouble { get; set; }


    //    [XmlElement("ValueReference")]
    //    public string ValueReference { get; set; }

    //}

    //public class TCareMessaging
    //{
    //    /// <summary>
    //    /// phương thức :
    //    /// interval : trước thời gian
    //    /// shedule : lên lịch ngày giờ cụ thể
    //    /// </summary>
    //    [XmlElement("MethodType")]
    //    public string MethodType { get; set; }

    //    /// <summary>
    //    /// MethodType : interval
    //    /// "minutes" , "hours" , "weeks", "months"
    //    /// </summary>
    //    [XmlElement("IntervalType")]
    //    public string IntervalType { get; set; }

    //    [XmlElement("IntervalNumber")]
    //    public int? IntervalNumber { get; set; }

    //    /// <summary>
    //    /// MethodType : shedule
    //    /// </summary>
    //    [XmlElement("SheduleDate")]
    //    public DateTime? SheduleDate { get; set; }


    //    [XmlElement("Content")]
    //    public string Content { get; set; }

    //    //--Kenh gui ---//

    //    /// <summary>
    //    ///  priority : ưu tiên
    //    ///  fixed : cố định
    //    /// </summary>
    //    [XmlElement("ChannelType")]
    //    public string ChannelType { get; set; }

    //    [XmlElement("ChannelSocialId")]
    //    public Guid? ChannelSocialId { get; set; }
    //}
}
