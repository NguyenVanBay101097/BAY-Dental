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

                    if (CampaignXML.Root.Sequence.MethodType == "interval")
                    {
                        var intervalNumber = int.Parse(CampaignXML.Root.Sequence.IntervalNumber);
                        if (CampaignXML.Root.Sequence.IntervalType == "hours")
                            date = date.AddHours(intervalNumber);
                        else if (CampaignXML.Root.Sequence.IntervalType == "minutes")
                            date = date.AddMinutes(intervalNumber);
                        else if (CampaignXML.Root.Sequence.IntervalType == "days")
                            date = date.AddDays(intervalNumber);
                        else if (CampaignXML.Root.Sequence.IntervalType == "months")
                            date = date.AddMonths(intervalNumber);
                        else if (CampaignXML.Root.Sequence.IntervalType == "weeks")
                            date = date.AddDays((intervalNumber) * 7);

                        var jobId = BackgroundJob.Schedule(() => SendMessageSocial(campaignId, db), date);
                        if (string.IsNullOrEmpty(jobId))
                            throw new Exception("Can't not schedule job");
                    }
                    else
                    {
                        date = DateTime.Parse(CampaignXML.Root.Sequence.SheduleDate);
                        var jobId = BackgroundJob.Schedule(() => SendMessageSocial(campaignId, db), date);
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



       



        public IEnumerable<Guid> SearchPartnerIds(IEnumerable<Condition> conditions, string typeRule, SqlConnection conn)
        {
            var partnerIds = new List<Guid>();
            var builder = new SqlBuilder();
            var lst = new Dictionary<string, string>();
            lst.Add("contains", "=");
            lst.Add("not_contains", "!=");
            //lst.Add("contains", "");
            //lst.Add("not_contains", "");
            lst.Add("lte", "<=");
            lst.Add("gte", ">=");
            var sqltemplate = builder.AddTemplate("SELECT pn.Id FROM Partners pn /**leftjoin**/ /**where**/ /**groupby**/ /**having**/ ");
            builder.Where("pn.Customer = 1 ");
            foreach (var condition in conditions)
            {
                switch (condition.Type)
                {
                    case "birthday":
                        var today = DateTime.Today;
                        var date = today.AddDays(int.Parse(condition.Value));
                        if (typeRule == "and")
                        {
                            builder.Where("pn.id in (Select pn.Id " +
                                "From Partners pn " +
                                "Where pn.BirthDay = @day AND pn.BirthMonth = @month) ", new { day = date.Day, month = date.Month });
                        }
                        else
                        {
                            builder.OrWhere("pn.id in (Select pn.Id " +
                                "From Partners pn " +
                                "Where pn.BirthDay = @day AND pn.BirthMonth = @month) ", new { day = date.Day, month = date.Month });
                        }

                        break;
                    case "lastSaleOrder":
                        //builder.LeftJoin("SaleOrders sale ON sale.PartnerId = pn.Id");
                        //builder.Where("sale.State = 'sale' ");
                        //builder.GroupBy(" pn.Id ");
                        //builder.Having("(Max(sale.DateOrder) <  DATEADD(day, -@number, GETDATE())) ", new { number = int.Parse(condition.Value) });
                        if (typeRule == "and")
                        {
                            builder.Where("pn.id in (Select pn.Id From Partners pn " +
                                "Left join SaleOrders sale ON sale.PartnerId = pn.Id " +
                                "Where sale.State = @sale " +
                                "Group by pn.Id " +
                                "Having (Max(sale.DateOrder) < DATEADD(day, -@number, GETDATE()))) ", new { number = int.Parse(condition.Value) , sale = "sale" });
                        }
                        else
                        {
                            builder.OrWhere("pn.id in (Select pn.Id From Partners pn " +
                                "Left join SaleOrders sale ON sale.PartnerId = pn.Id " +
                                "Where sale.State = @sale " +
                                "Group by pn.Id " +
                                "Having (Max(sale.DateOrder) < DATEADD(day, -@number, GETDATE()))) ", new { number = int.Parse(condition.Value), sale = "sale" });
                        }
                        break;
                    case "categPartner":
                        //builder.LeftJoin("PartnerPartnerCategoryRel rel On rel.PartnerId = pn.Id");
                        //builder.LeftJoin("PartnerCategories cpt On cpt.id = rel.CategoryId");
                        foreach (var kvp in lst.Where(x => x.Key == condition.Op))
                        {
                            switch (kvp.Key)
                            {
                                case "contains":
                                    if (typeRule == "and")
                                    {
                                        // builder.Where($"cpt.Id {kvp.Value} @cateId ", new { cateId = condition.Value });
                                        builder.Where("pn.id in (Select pn.Id " +
                                            "From Partners pn " +
                                            "Left join PartnerPartnerCategoryRel rel On rel.PartnerId = pn.Id " +
                                            "Left join PartnerCategories cpt On cpt.id = rel.CategoryId " +
                                            $"Where cpt.Id {kvp.Value} @cateId) ", new { cateId = condition.Value });
                                    }
                                    else
                                    {
                                        builder.OrWhere("pn.id in (Select pn.Id " +
                                           "From Partners pn " +
                                           "Left join PartnerPartnerCategoryRel rel On rel.PartnerId = pn.Id " +
                                           "Left join PartnerCategories cpt On cpt.id = rel.CategoryId " +
                                           $"Where cpt.Id {kvp.Value} @cateId) ", new { cateId = condition.Value });
                                    }
                                    break;
                                case "not_contains":
                                    if (typeRule == "and")
                                    {
                                        builder.Where("pn.id in (Select pn.Id " +
                                          "From Partners pn " +
                                          "Left join PartnerPartnerCategoryRel rel On rel.PartnerId = pn.Id " +
                                          "Left join PartnerCategories cpt On cpt.id = rel.CategoryId " +
                                          $"Where cpt.Id {kvp.Value} @cateId) ", new { cateId = condition.Value });
                                    }
                                    else
                                    {
                                        builder.OrWhere("pn.id in (Select pn.Id " +
                                           "From Partners pn " +
                                           "Left join PartnerPartnerCategoryRel rel On rel.PartnerId = pn.Id " +
                                           "Left join PartnerCategories cpt On cpt.id = rel.CategoryId " +
                                           $"Where cpt.Id {kvp.Value} @cateId) ", new { cateId = condition.Value });
                                    }

                                    break;
                                default:
                                    throw new NotSupportedException(string.Format("Not support Operator {0}!", condition.Value));
                            }
                        }

                        break;
                    case "usedService":

                        foreach (var kvp in lst.Where(x => x.Key == condition.Op))
                        {
                            switch (kvp.Key)
                            {
                                case "contains":
                                     if (typeRule == "and")
                                    {
                                        builder.Where("EXISTS (Select orlines.OrderPartnerId From SaleOrderLines orlines " +
                                           "Left join Products sp On sp.Id = orlines.ProductId " +
                                           $"Where sp.id = @serviceId And sp.Type2 = 'service' AND orlines.OrderPartnerId = pn.Id Group by orlines.OrderPartnerId) ", new { serviceId = condition.Value });
                                    }
                                    else
                                    {
                                        builder.OrWhere("EXISTS (Select orlines.OrderPartnerId From SaleOrderLines orlines " +
                                       "Left join Products sp On sp.Id = orlines.ProductId " +
                                       $"Where sp.id = @serviceId And sp.Type2 = 'service' AND orlines.OrderPartnerId = pn.Id Group by orlines.OrderPartnerId) ", new { serviceId = condition.Value });
                                    }

                                    break;
                                case "not_contains":
                                    if (typeRule == "and")
                                    {
                                        builder.Where("NOT EXISTS (Select orlines.OrderPartnerId From SaleOrderLines orlines " +
                                           "Left join Products sp On sp.Id = orlines.ProductId " +
                                           $"Where sp.id = @serviceId And sp.Type2 = 'service' AND orlines.OrderPartnerId = pn.Id Group by orlines.OrderPartnerId) ", new { serviceId = condition.Value });
                                    }
                                    else
                                    {
                                        builder.OrWhere("NOT EXISTS (Select orlines.OrderPartnerId From SaleOrderLines orlines " +
                                       "Left join Products sp On sp.Id = orlines.ProductId " +
                                       $"Where sp.id = @serviceId And sp.Type2 = 'service' AND orlines.OrderPartnerId = pn.Id Group by orlines.OrderPartnerId) ", new { serviceId = condition.Value });
                                    }

                                    break;
                                default:
                                    throw new NotSupportedException(string.Format("Not support Operator {0}!", condition.Value));
                            }
                        }
                        break;
                    case "usedCategService":
                        foreach (var kvp in lst.Where(x => x.Key == condition.Op))
                        {
                            switch (kvp.Key)
                            {
                                case "contains":
                                    if (typeRule == "and")
                                    {
                                        builder.Where("EXISTS (Select sale.PartnerId From SaleOrders sale " +
                                    "Left join SaleOrderLines orlines On orlines.OrderId = sale.Id " +
                                    "Left join Products sp On sp.Id = orlines.ProductId " +
                                    "Left join ProductCategories csp On csp.Id = sp.CategId " +
                                    $"Where csp.Id {kvp.Value} @groupservice And sp.Type2 = 'service' AND sale.PartnerId = pn.Id Group by sale.PartnerId) ", new { groupservice = condition.Value });
                                    }
                                    else
                                    {
                                        builder.OrWhere("EXISTS (Select sale.PartnerId From SaleOrders sale " +
                                    "Left join SaleOrderLines orlines On orlines.OrderId = sale.Id " +
                                    "Left join Products sp On sp.Id = orlines.ProductId " +
                                    "Left join ProductCategories csp On csp.Id = sp.CategId " +
                                    $"Where csp.Id {kvp.Value} @groupservice And sp.Type2 = 'service' AND sale.PartnerId = pn.Id Group by sale.PartnerId) ", new { groupservice = condition.Value });
                                    }

                                    break;
                                case "not_contains":
                                    if (typeRule == "and")
                                    {
                                        builder.Where("EXISTS (Select sale.PartnerId From SaleOrders sale " +
                                    "Left join SaleOrderLines orlines On orlines.OrderId = sale.Id " +
                                    "Left join Products sp On sp.Id = orlines.ProductId " +
                                    "Left join ProductCategories csp On csp.Id = sp.CategId " +
                                    $"Where csp.Id {kvp.Value} @groupservice And sp.Type2 = 'service' AND sale.PartnerId = pn.Id Group by sale.PartnerId) ", new { groupservice = condition.Value });
                                    }
                                    else
                                    {
                                        builder.OrWhere("EXISTS (Select sale.PartnerId From SaleOrders sale " +
                                    "Left join SaleOrderLines orlines On orlines.OrderId = sale.Id " +
                                    "Left join Products sp On sp.Id = orlines.ProductId " +
                                    "Left join ProductCategories csp On csp.Id = sp.CategId " +
                                    $"Where csp.Id {kvp.Value} @groupservice And sp.Type2 = 'service' AND sale.PartnerId = pn.Id Group by sale.PartnerId) ", new { groupservice = condition.Value });
                                    }
                                    break;
                                default:
                                    throw new NotSupportedException(string.Format("Not support Operator {0}!", condition.Value));
                            }
                        }

                        break;
                }

            }
            var partner_ids = conn.Query<Guid>(sqltemplate.RawSql, sqltemplate.Parameters).Distinct().ToList();
            return partner_ids;
        }

        public IEnumerable<Guid> SearchPartnerIdsUpdate(IEnumerable<Condition> conditions, string typeRule, SqlConnection conn)
        {
          
            var lstRule = new List<RulePartnerIds>();
            var partner_ids = new List<Guid>().AsEnumerable();
            var builder = new SqlBuilder();
            var lst = new Dictionary<string, string>();
            lst.Add("contains", "=");
            lst.Add("not_contains", "!=");
            //lst.Add("contains", "");
            //lst.Add("not_contains", "");
            lst.Add("lte", "<=");
            lst.Add("gte", ">=");        
            foreach (var condition in conditions)
            {
                switch (condition.Type)
                {
                    
                    case "birthday":
                        // lấy ra danh sách khách dựa vào sinh nhật của khách hàng
                        var today = DateTime.Today;
                        var date = today.AddDays(int.Parse(condition.Value));
                        var birthdayPartnerIds = conn.Query<RulePartnerIds>("" +
                                       "Select pn.Id " +
                                       "From Partners pn " +
                                       "Where pn.Customer = 1 and pn.BirthDay = @day AND pn.BirthMonth = @month ", new { day = date.Day, month = date.Month }
                                       ).FirstOrDefault();
                        lstRule.Add(birthdayPartnerIds);
                        break;
                    case "lastSaleOrder":
                        // lấy ra danh sách khách dựa vào phiểu điều trị cuối lấy ra ngày điều trị cuối của khách hàng
                        var lastSaleOrderPartnerIds = conn.Query<RulePartnerIds>("" +
                                "Select pn.Id From Partners pn " +
                                "Left join SaleOrders sale ON sale.PartnerId = pn.Id " +
                                "Where pn.Customer = 1 and sale.State = @sale " +
                                "Group by pn.Id " +
                                "Having (Max(sale.DateOrder) < DATEADD(day, -@number, GETDATE()))) ", new { number = int.Parse(condition.Value), sale = "sale" }).FirstOrDefault();
                        lstRule.Add(lastSaleOrderPartnerIds);
                        break;
                    case "categPartner":
                       // lấy ra danh sách khách dựa vào nhóm khách hàng
                        foreach (var kvp in lst.Where(x => x.Key == condition.Op))
                        {
                            switch (kvp.Key)
                            {
                                case "contains":
                                    //danh sách khách hàng thuộc nhóm khách hàng
                                    var categPartnerIds = conn.Query<RulePartnerIds>("" +
                                                   "Select pn.Id " +
                                                   "From Partners pn " +
                                                   "Left join PartnerPartnerCategoryRel rel On rel.PartnerId = pn.Id " +
                                                   "Left join PartnerCategories cpt On cpt.id = rel.CategoryId " +
                                                   $"Where pn.Customer = 1 and cpt.Id {kvp.Value} @cateId) ", new { cateId = condition.Value }).FirstOrDefault();
                                    lstRule.Add(categPartnerIds);
                                    break;
                                case "not_contains":
                                    //danh sách khách hàng không thuộc nhóm khách hàng
                                    var categNotPartnerIds = conn.Query<RulePartnerIds>("" +
                                                   "Select pn.Id " +
                                                   "From Partners pn " +
                                                   "Left join PartnerPartnerCategoryRel rel On rel.PartnerId = pn.Id " +
                                                   "Left join PartnerCategories cpt On cpt.id = rel.CategoryId " +
                                                   $"Where pn.Customer = 1 and cpt.Id {kvp.Value} @cateId) ", new { cateId = condition.Value }).FirstOrDefault();
                                    lstRule.Add(categNotPartnerIds);
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
                                    var usedServicePartnerIds = conn.Query<RulePartnerIds>("" +
                                    "Select pn.Id " +
                                    "From Partners pn " +
                                    "Where pn.Customer = 1 and EXISTS (Select orlines.OrderPartnerId From SaleOrderLines orlines " +
                                           "Left join Products sp On sp.Id = orlines.ProductId " +
                                           $"Where sp.id = @serviceId And sp.Type2 = 'service' AND orlines.OrderPartnerId = pn.Id Group by orlines.OrderPartnerId) ", new { serviceId = condition.Value })
                                        .FirstOrDefault();
                                    lstRule.Add(usedServicePartnerIds);
                                    break;
                                case "not_contains":
                                    //danh sách khách hàng chưa sử dụng dịch vụ 
                                    var usedServiceNotPartnerIds = conn.Query<RulePartnerIds>("" +
                                   "Select pn.Id " +
                                   "From Partners pn " +
                                   "Where pn.Customer = 1 and NOT EXISTS (Select orlines.OrderPartnerId From SaleOrderLines orlines " +
                                          "Left join Products sp On sp.Id = orlines.ProductId " +
                                          $"Where sp.id = @serviceId And sp.Type2 = 'service' AND orlines.OrderPartnerId = pn.Id Group by orlines.OrderPartnerId) ", new { serviceId = condition.Value })
                                       .FirstOrDefault();
                                    lstRule.Add(usedServiceNotPartnerIds);                                  
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
                                    var usedCategServicePartnerIds = conn.Query<RulePartnerIds>("" + "Select pn.IdFrom Partners pn " +
                                        "Where pn.Customer = 1 and EXISTS(Select orlines.OrderPartnerId " +
                                        "From SaleOrderLines orlines " +
                                        "Left join Products sp On sp.Id = orlines.ProductId " +
                                        "Left join ProductCategories csp On csp.Id = sp.CategId " +
                                        "Where csp.Id = @categService And sp.Type2 = 'service' AND orlines.OrderPartnerId = pn.Id Group by orlines.OrderPartnerId) ", new { categService = condition.Value }).FirstOrDefault();

                                    lstRule.Add(usedCategServicePartnerIds);
                                    break;
                                case "not_contains":
                                    //danh sách khách hàng chưa sử dụng nhóm dịch vụ
                                    var usedCategServiceNotPartnerIds = conn.Query<RulePartnerIds>("" + "Select pn.IdFrom Partners pn " +
                                       "Where pn.Customer = 1 and NOT EXISTS(Select orlines.OrderPartnerId " +
                                       "From SaleOrderLines orlines " +
                                       "Left join Products sp On sp.Id = orlines.ProductId " +
                                       "Left join ProductCategories csp On csp.Id = sp.CategId " +
                                       "Where csp.Id = @categService And sp.Type2 = 'service' AND orlines.OrderPartnerId = pn.Id Group by orlines.OrderPartnerId) ", new { categService = condition.Value }).FirstOrDefault();
                                    lstRule.Add(usedCategServiceNotPartnerIds);
                                    break;
                                default:
                                    throw new NotSupportedException(string.Format("Not support Operator {0}!", condition.Value));
                            }
                        }

                        break;
                }

            }
            if (typeRule == "and")
            {
                foreach (var rule in lstRule)
                    partner_ids = partner_ids.Intersect(rule.Ids).Distinct().ToList();

            }
            else 
            {

            }
                //var partner_ids = conn.Query<Guid>(sqltemplate.RawSql, sqltemplate.Parameters).Distinct().ToList();
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
                    partner_ids = SearchPartnerIdsUpdate(resultingMessage.Root.Rule.Condition, resultingMessage.Root.Rule.Logic, conn);

                    //Get partnerIds in list rules

                    //if (partner_ids.Count() == 0)
                    //    continue;

                    //var messaging = conn.Query<TCareMessaging>("SELECT * FROM TCareMessagings WHERE TCareCampaignId = @id", new { id = campaignId }).FirstOrDefault();
                    if (resultingMessage.Root.Sequence.Content == null)
                        return;

                    var channelSocial = conn.Query<FacebookPage>("" +
                         "SELECT * " +
                         "FROM FacebookPages " +
                         "where Id = @id" +
                         "", new { id = resultingMessage.Root.Sequence.ChannelSocialId }).FirstOrDefault();

                    if (channelSocial == null)
                        return;

                    profiles = GetUserProfiles(Guid.Parse(resultingMessage.Root.Sequence.ChannelSocialId), partner_ids, conn);
                    if (profiles == null)
                        return;
                    if (channelSocial.Type == "facebook")
                    {
                        var tasks = profiles.Select(x => SendMessageAndTrace(conn, resultingMessage.Root.Sequence.Content, x, channelSocial.PageAccesstoken, Guid.Parse(resultingMessage.Root.Sequence.CampaignId))).ToList();
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

                        var tasks = profiles.Select(x => SendMessageAndTraceZalo(conn, resultingMessage.Root.Sequence.Content, x, channelSocial.PageAccesstoken, Guid.Parse(resultingMessage.Root.Sequence.CampaignId))).ToList();
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
                    if (resultingMessage.Root.Sequence.ChannelType == "priority")
                    {
                        var channelSocials = conn.Query<FacebookPage>("" +
                             "SELECT * " +
                             "FROM FacebookPages " +
                             "where Id != @id" +
                             "", new { id = resultingMessage.Root.Sequence.ChannelSocialId }).ToList();

                        foreach (var channel in channelSocials)
                        {
                            if (partner_ids.Count() == 0)
                                break;
                            profiles = GetUserProfiles(channel.Id, partner_ids, conn);
                            if (profiles == null)
                                return;
                            if (channel.Type == "facebook")
                            {
                                var tasks = profiles.Select(x => SendMessageAndTrace(conn, resultingMessage.Root.Sequence.Content, x, channel.PageAccesstoken, Guid.Parse(resultingMessage.Root.Sequence.CampaignId))).ToList();
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

                                var tasks = profiles.Select(x => SendMessageAndTraceZalo(conn, resultingMessage.Root.Sequence.Content, x, channel.PageAccesstoken, Guid.Parse(resultingMessage.Root.Sequence.CampaignId))).ToList();
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


        private static IEnumerable<FacebookUserProfile> GetUserProfiles(Guid ChannelSocialId, IEnumerable<Guid> partIds,
           SqlConnection conn = null)
        {
            var builder = new SqlBuilder();
            var sqltemplate = builder.AddTemplate("SELECT /**select**/ FROM FacebookUserProfiles us /**leftjoin**/ /**where**/ /**orderby**/ ");

            builder.Select("us.id , us.PSID , us.Name , us.PartnerId ");
            builder.Where("us.FbPageId = @PageId ", new { PageId = ChannelSocialId });
            var iUserprofiles = conn.Query<FacebookUserProfile>(sqltemplate.RawSql, sqltemplate.Parameters).ToList();
            // lấy danh sách userprofiles theo filter
            //var profiles = GetProfilesActivity(activity, pageId, conn);
            var lstUserProfile = iUserprofiles.Where(x => partIds.Any(s => s == x.PartnerId)).ToList();

            return lstUserProfile;

        }



        public async Task SendMessageAndTrace(SqlConnection conn, string text, FacebookUserProfile profile, string access_token, Guid campaignId)
        {
            var now = DateTime.Now;
            var date = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            date = date.AddSeconds(-1);

            var sendResult = await _fbMessageSender.SendMessageTCareTextAsync(text, profile.PSID, access_token);
            if (sendResult == null)
                await conn.ExecuteAsync("insert into TCareMessingTraces(Id,Exception,TCareCampaignId,PSID,PartnerId,Type) Values (@Id,@Exception,@TCareCampaignId,@PSID,@PartnerId,@Type)", new { Id = GuidComb.GenerateComb(), Exception = date, TCareCampaignId = campaignId, PSID = profile.PSID, PartnerId = profile.PartnerId, Type = "facebook" });
            else
                await conn.ExecuteAsync("insert into TCareMessingTraces(Id,Sent,TCareCampaignId,MessageId,PSID,PartnerId,Type) Values (@Id,@Sent,@TCareCampaignId,@MessageId,@PSID,@PartnerId,@Type)", new { Id = GuidComb.GenerateComb(), Sent = date, TCareCampaignId = campaignId, MessageId = sendResult.message_id, PSID = profile.PSID, PartnerId = profile.PartnerId, Type = "facebook" });

        }

        public async Task SendMessageAndTraceZalo(SqlConnection conn, string text, FacebookUserProfile profile, string access_token, Guid campaignId)
        {
            var now = DateTime.Now;
            var date = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            date = date.AddSeconds(-1);
            var zaloClient = new ZaloClient(access_token);
            var sendResult = zaloClient.sendTextMessageToUserId(profile.PSID, text).Root.ToObject<RootZalo>().data;
            if (sendResult == null)
                await conn.ExecuteAsync("insert into TCareMessingTraces(Id,Exception,TCareCampaignId,PSID,PartnerId,Type) Values (@Id,@Exception,@TCareCampaignId,@PSID,@PartnerId,@Type)", new { Id = GuidComb.GenerateComb(), Exception = date, TCareCampaignId = campaignId, PSID = profile.PSID, PartnerId = profile.PartnerId, Type = "zalo" });
            else
                await conn.ExecuteAsync("insert into TCareMessingTraces(Id,Sent,TCareCampaignId,MessageId,PSID,PartnerId,Type) Values (@Id,@Sent,@TCareCampaignId,@MessageId,@PSID,@PartnerId,@Type)", new { Id = GuidComb.GenerateComb(), Sent = date, TCareCampaignId = campaignId, MessageId = sendResult.message_id, PSID = profile.PSID, PartnerId = profile.PartnerId, Type = "zalo" });


        }


    }



    public class PartnerSendMessageResult
    {
        public string ZaloId { get; set; }
        public string Content { get; set; }
    }



    public class RootZalo
    {
        public RootMessageZalo data { get; set; }
    }

    public class RootMessageZalo
    {
        public string message_id { get; set; }
    }

    public class RulePartnerIds
    {
        public List<Guid> Ids { get; set; }
    }

}
