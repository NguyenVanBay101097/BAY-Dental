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
            builder["Database"] = $"TMTDentalCatalogDb__{db}";
            if (db == "localhost")
                builder["Database"] = "TMTDentalCatalogDb";
            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    conn.Open();
                    var date = DateTime.UtcNow;
                    var campaign = conn.Query<TCareCampaign>("SELECT * FROM TCareCampaigns WHERE Id = @id", new { id = campaignId }).FirstOrDefault();

                    //var campaignXml = ConvertXmlCampaign(campaign.GraphXml);
                    XmlSerializer serializer = new XmlSerializer(typeof(MxGraphModel));
                    MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(campaign.GraphXml));
                    MxGraphModel CampaignXML = (MxGraphModel)serializer.Deserialize(memStream);
                    //var messaging = conn.Query<TCareMessaging>("SELECT * FROM TCareMessagings WHERE TCareCampaignId = @id", new { id = campaign.Id }).FirstOrDefault();

                    if (CampaignXML.Root.Sequences.Methodtype == "interval")
                    {
                        var intervalNumber = int.Parse(CampaignXML.Root.Sequences.Intervalnumber);
                        if (CampaignXML.Root.Sequences.Intervaltype == "hours")
                            date = date.AddHours(intervalNumber);
                        else if (CampaignXML.Root.Sequences.Intervaltype == "minutes")
                            date = date.AddMinutes(intervalNumber);
                        else if (CampaignXML.Root.Sequences.Intervaltype == "days")
                            date = date.AddDays(intervalNumber);
                        else if (CampaignXML.Root.Sequences.Intervaltype == "months")
                            date = date.AddMonths(intervalNumber);
                        else if (CampaignXML.Root.Sequences.Intervaltype == "weeks")
                            date = date.AddDays((intervalNumber) * 7);

                        var jobId = BackgroundJob.Schedule(() => SendMessageSocial(campaignId,db), date);
                        if (string.IsNullOrEmpty(jobId))
                            throw new Exception("Can't not schedule job");
                    }
                    else
                    {                       
                        date = DateTime.Parse(CampaignXML.Root.Sequences.Sheduledate);
                        var jobId = BackgroundJob.Schedule(() => SendMessageSocial(campaignId,db), date);
                        if (string.IsNullOrEmpty(jobId))
                            throw new Exception("Can't not schedule job");
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }



        //public CampaignXml ConvertXmlCampaign(string xmlFilePath)
        //{
        //    XmlSerializer serializer = new XmlSerializer(typeof(MxGraphModel));
        //    MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlFilePath));
        //    MxGraphModel resultingMessage = (MxGraphModel)serializer.Deserialize(memStream);

        //    //if(resultingMessage == null)
        //    //    throw new Exception("")

        //    var campaignXml = new CampaignXml
        //    {
        //        RuleXml = new RuleXml
        //        {
        //            Condition = resultingMessage.Root.Rule.Condition,
        //            Logic = resultingMessage.Root.Rule.Logic,
        //        },
        //        MessageXML = new MessageXML
        //        {
        //            Tcarecampaignid = resultingMessage.Root.Sequences.Tcarecampaignid,
        //            Parentid = resultingMessage.Root.Sequences.Parentid,
        //            Messagereadid = resultingMessage.Root.Sequences.Messagereadid,
        //            Messageunreadid = resultingMessage.Root.Sequences.Messageunreadid,
        //            Content = resultingMessage.Root.Sequences.Content,
        //            Channeltype = resultingMessage.Root.Sequences.Channeltype,
        //            Intervalnumber = int.Parse(resultingMessage.Root.Sequences.Intervalnumber),
        //            Intervaltype = resultingMessage.Root.Sequences.Intervaltype,
        //            Methodtype = resultingMessage.Root.Sequences.Methodtype,
        //            Channelsocialid = Guid.Parse(resultingMessage.Root.Sequences.ChannelsocialId),

        //        }
        //    };


        //    return campaignXml;
        //}


        /// <summary>
        /// check Rule And
        /// </summary>
        /// <param name="rules"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public IEnumerable<Guid> SearchAndPartnerIds(IEnumerable<Condition> conditions, SqlConnection conn)
        {
            var partnerIds = new List<Guid>();
            var builder = new SqlBuilder();
            var lst = new Dictionary<string, string>();
            lst.Add("eq", "=");
            lst.Add("neq", "!=");
            var sqltemplate = builder.AddTemplate("SELECT /**select**/ FROM Partners pn /**leftjoin**/ /**where**/ /**groupby**/ /**having**/ ");
            builder.Select("pn.Id ");
            builder.Where("pn.Customer = 1 ");
            foreach (var condition in conditions)
            {
                switch (condition.Typecondition)
                {
                    case "birthday":
                        var today = DateTime.Today;
                        var date = today.AddDays(int.Parse(condition.Valuecondition));
                        builder.Where("pn.BirthDay = @day AND pn.BirthMonth = @month ", new { day = date.Day, month = date.Month });
                        //var partner_ids = conn.Query<Guid>("SELECT Id FROM Partners WHERE Customer = 1 AND BirthDay = @day AND BirthMonth = @month", new { day = date.Day, month = date.Month }).ToList();
                        // var partner_ids = conn.Query<Guid>(sqltemplate.RawSql, sqltemplate.Parameters).ToList();     
                        break;
                    case "lastTreatmentDay":
                        builder.LeftJoin("SaleOrders sale ON sale.PartnerId = pn.Id");
                        builder.Where("sale.State = 'sale' ");
                        builder.GroupBy(" pn.Id ");
                        builder.Having("(Max(sale.DateOrder) <  DATEADD(day, -@number, GETDATE())) ", new { number = int.Parse(condition.Valuecondition) });
                        break;
                    case "groupPartner":
                        builder.LeftJoin("PartnerPartnerCategoryRel rel On rel.PartnerId = pn.Id");
                        builder.LeftJoin("PartnerCategories cpt On cpt.id = rel.CategoryId");
                        foreach (var kvp in lst.Where(x => x.Key == condition.Flagcondition))
                        {
                            switch (kvp.Key)
                            {
                                case "eq":
                                    builder.Where($"cpt.Id {kvp.Value} @cateId ", new { cateId = condition.Valuecondition });
                                    break;
                                case "neq":
                                    builder.Where($"cpt.Id {kvp.Value} @cateId ", new { cateId = condition.Valuecondition });
                                    break;
                                default:
                                    throw new NotSupportedException(string.Format("Not support Operator {0}!", condition.Valuecondition));
                            }
                        }
                        break;
                    case "service":

                        foreach (var kvp in lst.Where(x => x.Key == condition.Flagcondition))
                        {
                            switch (kvp.Key)
                            {
                                case "eq":
                                    builder.Where("EXISTS (Select sale.PartnerId From SaleOrders sale " +
                                        "Left join SaleOrderLines orlines On orlines.OrderId = sale.Id " +
                                        "Left join Products sp On sp.Id = orlines.ProductId " +
                                        $"Where sp.id {kvp.Value} @serviceId And sp.Type2 = 'service' AND sale.PartnerId = pn.Id Group by sale.PartnerId) ", new { serviceId = condition.Valuecondition });
                                    break;
                                case "neq":
                                    builder.Where("EXISTS (Select sale.PartnerId From SaleOrders sale " +
                                        "Left join SaleOrderLines orlines On orlines.OrderId = sale.Id " +
                                        "Left join Products sp On sp.Id = orlines.ProductId " +
                                        $"Where sp.id {kvp.Value} @serviceId And sp.Type2 = 'service' AND sale.PartnerId = pn.Id Group by sale.PartnerId) ", new { serviceId = condition.Valuecondition });
                                    break;
                                default:
                                    throw new NotSupportedException(string.Format("Not support Operator {0}!", condition.Valuecondition));
                            }
                        }
                        break;
                    case "groupService":
                        foreach (var kvp in lst.Where(x => x.Key == condition.Flagcondition))
                        {
                            switch (kvp.Key)
                            {
                                case "eq":
                                    builder.Where("EXISTS (Select sale.PartnerId From SaleOrders sale " +
                                    "Left join SaleOrderLines orlines On orlines.OrderId = sale.Id " +
                                    "Left join Products sp On sp.Id = orlines.ProductId " +
                                    "Left join ProductCategories csp On csp.Id = sp.CategId " +
                                    $"Where csp.Id {kvp.Value} @groupservice And sp.Type2 = 'service' AND sale.PartnerId = pn.Id Group by sale.PartnerId) ", new { groupservice = condition.Valuecondition });
                                    break;
                                case "neq":
                                    builder.Where("EXISTS (Select sale.PartnerId From SaleOrders sale " +
                                        "Left join SaleOrderLines orlines On orlines.OrderId = sale.Id " +
                                        "Left join Products sp On sp.Id = orlines.ProductId " +
                                        "Left join ProductCategories csp On csp.Id = sp.CategId " +
                                        $"Where csp.Id {kvp.Value} @groupservice And sp.Type2 = 'service' AND sale.PartnerId = pn.Id Group by sale.PartnerId) ", new { groupservice = condition.Valuecondition });
                                    break;
                                default:
                                    throw new NotSupportedException(string.Format("Not support Operator {0}!", condition.Valuecondition));
                            }
                        }
                        break;
                }

            }
            var partner_ids = conn.Query<Guid>(sqltemplate.RawSql, sqltemplate.Parameters).Distinct().ToList();
            return partner_ids;
        }

        /// <summary>
        /// check Rule Or
        /// </summary>
        /// <param name="rules"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public IEnumerable<Guid> SearchOrPartnerIds(IEnumerable<Condition> conditions, SqlConnection conn)
        {
            var partnerIds = new List<Guid>();
            var builder = new SqlBuilder();
            var lst = new Dictionary<string, string>();
            lst.Add("eq", "=");
            lst.Add("neq", "!=");
            var sqltemplate = builder.AddTemplate("SELECT /**select**/ FROM Partners pn /**leftjoin**/ /**where**/ /**groupby**/ /**having**/ ");
            builder.Select("pn.Id ");
            builder.Where("pn.Customer = 1 ");
            foreach (var condition in conditions)
            {
                if (condition.Typecondition == "birthday")
                {
                    var today = DateTime.Today;
                    var date = today.AddDays(int.Parse(condition.Valuecondition));
                    builder.OrWhere("pn.BirthDay = @day AND pn.BirthMonth = @month ", new { day = date.Day, month = date.Month });
                    //var partner_ids = conn.Query<Guid>("SELECT Id FROM Partners WHERE Customer = 1 AND BirthDay = @day AND BirthMonth = @month", new { day = date.Day, month = date.Month }).ToList();
                    // var partner_ids = conn.Query<Guid>(sqltemplate.RawSql, sqltemplate.Parameters).ToList();          
                }
                else if (condition.Typecondition == "lastTreatmentDay")
                {
                    builder.LeftJoin("left join SaleOrders sale ON sale.PartnerId = pn.Id");
                    builder.Where("sale.State = 'sale' ");
                    builder.GroupBy("pn.Id ");
                    builder.Having("(Max(sale.DateOrder) <  DATEADD(day, -@number, GETDATE())) ", new { number = int.Parse(condition.Valuecondition) });

                }
                else if (condition.Typecondition == "groupPartner")
                {
                    builder.LeftJoin("PartnerPartnerCategoryRel rel On rel.PartnerId = pn.Id");
                    builder.LeftJoin("PartnerCategories cpt On cpt.id = rel.CategoryId");
                    foreach (var kvp in lst.Where(x => x.Key == condition.Flagcondition))
                    {
                        switch (kvp.Key)
                        {
                            case "eq":
                                builder.OrWhere($"cpt.Id {kvp.Value} @cateId ", new { cateId = condition.Valuecondition });
                                break;
                            case "neq":
                                builder.OrWhere($"cpt.Id {kvp.Value} @cateId ", new { cateId = condition.Valuecondition });
                                break;
                            default:
                                throw new NotSupportedException(string.Format("Not support Operator {0}!", condition.Valuecondition));
                        }
                    }
                }
                else if (condition.Typecondition == "service")
                {
                    foreach (var kvp in lst.Where(x => x.Key == condition.Flagcondition))
                    {
                        switch (kvp.Key)
                        {
                            case "eq":
                                builder.OrWhere("EXISTS (Select sale.PartnerId From SaleOrders sale " +
                                    "Left join SaleOrderLines orlines On orlines.OrderId = sale.Id " +
                                    "Left join Products sp On sp.Id = orlines.ProductId " +
                                    $"Where sp.id {kvp.Value} @serviceId And sp.Type2 = 'service' AND sale.PartnerId = pn.Id Group by sale.PartnerId) ", new { serviceId = condition.Valuecondition });
                                break;
                            case "neq":
                                builder.OrWhere("EXISTS (Select sale.PartnerId From SaleOrders sale " +
                                    "Left join SaleOrderLines orlines On orlines.OrderId = sale.Id " +
                                    "Left join Products sp On sp.Id = orlines.ProductId " +
                                    $"Where sp.id {kvp.Value} @serviceId And sp.Type2 = 'service' AND sale.PartnerId = pn.Id Group by sale.PartnerId) ", new { serviceId = condition.Valuecondition });
                                break;
                            default:
                                throw new NotSupportedException(string.Format("Not support Operator {0}!", condition.Valuecondition));
                        }
                    }
                }
                else if (condition.Typecondition == "groupService")
                {
                    foreach (var kvp in lst.Where(x => x.Key == condition.Flagcondition))
                    {
                        switch (kvp.Key)
                        {
                            case "eq":
                                builder.OrWhere("EXISTS (Select sale.PartnerId From SaleOrders sale " +
                                "Left join SaleOrderLines orlines On orlines.OrderId = sale.Id " +
                                "Left join Products sp On sp.Id = orlines.ProductId " +
                                "Left join ProductCategories csp On csp.Id = sp.CategId " +
                                $"Where csp.Id {kvp.Value} @groupserviceId And sp.Type2 = 'service' AND sale.PartnerId = pn.Id Group by sale.PartnerId) ", new { groupserviceId = condition.Valuecondition });
                                break;
                            case "neq":
                                builder.OrWhere("EXISTS (Select sale.PartnerId From SaleOrders sale " +
                                    "Left join SaleOrderLines orlines On orlines.OrderId = sale.Id " +
                                    "Left join Products sp On sp.Id = orlines.ProductId " +
                                    "Left join ProductCategories csp On csp.Id = sp.CategId " +
                                    $"Where csp.Id {kvp.Value} @groupserviceId And sp.Type2 = 'service' AND sale.PartnerId = pn.Id Group by sale.PartnerId) ", new { groupserviceId = condition.Valuecondition });
                                break;
                            default:
                                throw new NotSupportedException(string.Format("Not support Operator {0}!", condition.Valuecondition));
                        }
                    }
                }
            }
            var partner_ids = conn.Query<Guid>(sqltemplate.RawSql, sqltemplate.Parameters).Distinct().ToList();
            return partner_ids;
        }
        public async Task SendMessageSocial(Guid? campaignId = null,
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

                    var partner_ids = new List<Guid>().AsEnumerable();
                    var profiles = new List<FacebookUserProfile>().AsEnumerable();
                    var campaign = conn.Query<TCareCampaign>("SELECT * FROM TCareCampaigns WHERE Id = @id", new { id = campaignId }).FirstOrDefault();
                    if (campaign == null)
                        return;
                    //var campaignXml = ConvertXmlCampaign(campaign.GraphXml);
                    XmlSerializer serializer = new XmlSerializer(typeof(MxGraphModel));
                    MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(campaign.GraphXml));
                    MxGraphModel resultingMessage = (MxGraphModel)serializer.Deserialize(memStream);
                    /// kiem tra dieu kien  and hoac or
                    if (resultingMessage.Root.Rule.Logic == "and")
                    {
                        partner_ids = SearchAndPartnerIds(resultingMessage.Root.Rule.Condition, conn);
                    }
                    else
                    {
                        partner_ids = SearchOrPartnerIds(resultingMessage.Root.Rule.Condition, conn);
                    }



                    //Get partnerIds in list rules

                    //if (partner_ids.Count() == 0)
                    //    continue;

                    //var messaging = conn.Query<TCareMessaging>("SELECT * FROM TCareMessagings WHERE TCareCampaignId = @id", new { id = campaignId }).FirstOrDefault();
                    if (resultingMessage.Root.Sequences.Content == null)
                        return;

                    var channelSocial = conn.Query<FacebookPage>("" +
                         "SELECT * " +
                         "FROM FacebookPages " +
                         "where Id = @id" +
                         "", new { id = resultingMessage.Root.Sequences.ChannelsocialId }).FirstOrDefault();

                    if (channelSocial == null)
                        return;

                    profiles = GetUserProfiles(Guid.Parse(resultingMessage.Root.Sequences.ChannelsocialId), partner_ids, conn);
                    if (profiles == null)
                        return;
                    if (channelSocial.Type == "facebook")
                    {
                        var tasks = profiles.Select(x => SendMessageAndTrace(conn, resultingMessage.Root.Sequences.Content, x, channelSocial.PageAccesstoken, Guid.Parse(resultingMessage.Root.Sequences.Tcarecampaignid))).ToList();
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
                       
                        var tasks = profiles.Select(x => SendMessageAndTraceZalo(conn, resultingMessage.Root.Sequences.Content, x, channelSocial.PageAccesstoken, Guid.Parse(resultingMessage.Root.Sequences.Tcarecampaignid))).ToList();
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

                    //lấy ra partnerids của danh sách profiles mới gửi
                    partner_ids = partner_ids.Where(x => !profiles.Any(s => s.PartnerId == x)).ToList();

                    // check điều kiện kênh ưu tiên
                    if (resultingMessage.Root.Sequences.Channeltype == "priority")
                    {
                        var channelSocials = conn.Query<FacebookPage>("" +
                             "SELECT * " +
                             "FROM FacebookPages " +
                             "where Id != @id" +
                             "", new { id = resultingMessage.Root.Sequences.ChannelsocialId }).ToList();

                        foreach (var channel in channelSocials)
                        {
                            if (partner_ids.Count() == 0)
                                break;
                            profiles = GetUserProfiles(channel.Id, partner_ids, conn);
                            if (profiles == null)
                                return;
                            if (channel.Type == "facebook")
                            {
                                var tasks = profiles.Select(x => SendMessageAndTrace(conn, resultingMessage.Root.Sequences.Content, x, channel.PageAccesstoken, Guid.Parse(resultingMessage.Root.Sequences.Tcarecampaignid))).ToList();
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
                                
                                var tasks = profiles.Select(x => SendMessageAndTraceZalo(conn, resultingMessage.Root.Sequences.Content, x, channel.PageAccesstoken, Guid.Parse(resultingMessage.Root.Sequences.Tcarecampaignid))).ToList();
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
            var lstUserProfile = iUserprofiles.Where(x => partIds.Any(s => s == x.PartnerId)).ToList();

            return lstUserProfile;

        }



        public async Task SendMessageAndTrace(SqlConnection conn, string text, FacebookUserProfile profile, string access_token , Guid campaignId)
        {
            var now = DateTime.Now;
            var date = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            date = date.AddSeconds(-1);
           
            var sendResult = await _fbMessageSender.SendMessageTCareTextAsync(text, profile.PSID, access_token);
            if (sendResult == null)
                await conn.ExecuteAsync("insert into TCareMessingTraces(Id,Exception,TCareCampaignId,PartnerId,Type) values (@Id,@Exception,@TCareCampaignId,@PartnerId,@Type)", new { Id = GuidComb.GenerateComb(), Exception = date, TCareCampaignId = campaignId, PartnerId = profile.PartnerId, Type = "facebook" });
            else
                await conn.ExecuteAsync("insert into TCareMessingTraces(Id,Sent,TCareCampaignId,MessageId,PartnerId,Type) values (@Id,@Sent,@TCareCampaignId,@MessageId,@PartnerId,@Type)", new { Id = GuidComb.GenerateComb(), Sent = date, TCareCampaignId = campaignId, MessageId = sendResult.message_id, PartnerId = profile.PartnerId, Type = "facebook" });
           

        }

        public async Task SendMessageAndTraceZalo(SqlConnection conn, string text, FacebookUserProfile profile, string access_token, Guid campaignId)
        {
            var now = DateTime.Now;
            var date = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            date = date.AddSeconds(-1);
            var zaloClient = new ZaloClient(access_token);
            var sendResult = zaloClient.sendTextMessageToUserId( profile.PSID, text).Root.ToObject<RootZalo>().data;
            if (sendResult == null)
                await conn.ExecuteAsync("insert into TCareMessingTraces(Id,Exception,TCareCampaignId,PartnerId,Type) values (@Id,@Exception,@TCareCampaignId,@PartnerId,@Type)", new { Id = GuidComb.GenerateComb(), Exception = date, TCareCampaignId = campaignId, PartnerId = profile.PartnerId, Type = "zalo" });
            else
                await conn.ExecuteAsync("insert into TCareMessingTraces(Id,Sent,TCareCampaignId,MessageId,PartnerId,Type) values (@Id,@Sent,@TCareCampaignId,@MessageId,@PartnerId,@Type)", new { Id = GuidComb.GenerateComb(), Sent = date, TCareCampaignId = campaignId, MessageId = sendResult.message_id, PartnerId = profile.PartnerId, Type = "zalo" });


        }


    }



    public class PartnerSendMessageResult
    {
        public string ZaloId { get; set; }
        public string Content { get; set; }
    }

    public class CampaignXml
    {
        public RuleXml RuleXml { get; set; }

        public MessageXML MessageXML { get; set; }
    }

    public class RuleXml
    {
        public List<Condition> Condition { get; set; } = new List<Condition>();
        public string Logic { get; set; }
    }

    public class RootZalo
    {
        public RootMessageZalo data { get; set; }
    }

    public class RootMessageZalo
    {
        public string message_id { get; set; }
    }


    public class MessageXML
    {

        public string Tcarecampaignid { get; set; }

        public string Parentid { get; set; }

        public string Messagereadid { get; set; }

        public string Messageunreadid { get; set; }

        public Guid? Channelsocialid { get; set; }

        public string Channeltype { get; set; }

        public string Content { get; set; }

        public int? Intervalnumber { get; set; }

        public string Intervaltype { get; set; }

        public string Methodtype { get; set; }

        public DateTime? Sheduledate { get; set; }

        public string Id { get; set; }
        public string Xmlns { get; set; }
    }
}
