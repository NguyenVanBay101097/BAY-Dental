using ApplicationCore.Entities;
using Dapper;
using Infrastructure.Data;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public void RunActivity(string db, Guid activityId)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.CatalogConnection);
            builder["Database"] = $"TMTDentalCatalogDb__{db}";
            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    conn.Open();
                    var activity = conn.Query<MarketingCampaignActivity>("SELECT * FROM MarketingCampaignActivities").FirstOrDefault();
                    if (activity != null)
                    {
                        var list = new List<PartnerSendMessageResult>();
                        if (activity.Condition == "no_sales")
                        {
                            var date = activity.Campaign.DateStart ?? DateTime.Now;
                            var result = conn.Query<PartnerSendMessageResult>("select p.ZaloId as ZaloId " +
                                "from Partners p " +
                                "inner join(" +
                                    "select s.PartnerId, MAX(s.DateOrder) as LastDateOrder from SaleOrders s " +
                                    "group by s.PartnerId) sub on sub.PartnerId = p.Id " +
                                "where sub.LastDateOrder < @date", new { date }).ToList();

                            list = result.Select(x => new PartnerSendMessageResult
                            {
                                Content = x.Content,
                                ZaloId = x.ZaloId
                            }).ToList();
                        }
                        //else if (activity.Condition == "birthday")
                        //{
                        //    var today = DateTime.Today;
                        //    var result = conn.Query<PartnerSendMessageResult>(@"SELECT p.ZaloId as ZaloId FROM Partners p WHERE p.ZaloId IS NOT NULL AND p.Customer = 1 AND p.BirthMonth = @month AND p.BirthDay = @day",
                        //                new { month = today.Month, day = today.Day }).ToList();

                        //    list = result.Select(x => new PartnerSendMessageResult
                        //    {
                        //        Content = x.Content,
                        //        ZaloId = x.ZaloId
                        //    }).ToList();
                        //}
                        //else if (activity.Condition == "today_appointment")
                        //{
                        //    var result = conn.Query<PartnerToDayAppointmentQuery>(@"select ap.Date as Date, p.Name as PartnerName, p.ZaloId as PartnerZaloId from Appointments ap " +
                        //                                                "left join Partners p on ap.PartnerId = p.Id " +
                        //                                                "where DATEPART(DAY, ap.Date) = DATEPART(day, GETDATE()) and " +
                        //                                                "DATEPART(MONTH, ap.Date) = DATEPART(day, GETDATE()) and " +
                        //                                                "DATEPART(YEAR, ap.Date) = DATEPART(YEAR, GETDATE())").ToList();
                        //    foreach (var item in result)
                        //    {
                        //        list.Add(new PartnerSendMessageResult
                        //        {
                        //            ZaloId = item.PartnerZaloId,
                        //            Content = Regex.Replace(activity.Content, "{thoi_gian_lich_hen}", item.Date.ToString("HH:mm")),
                        //        });
                        //    }
                        //}

                        if (list.Any())
                        {
                            if (activity.ActivityType == "zalo")
                            {
                                var zaloConfig = conn.Query<ZaloOAConfig>("SELECT * FROM ZaloOAConfigs").FirstOrDefault();
                                if (zaloConfig != null)
                                {
                                    var zaloClient = new ZaloClient(zaloConfig.AccessToken);
                                    foreach (var item in list)
                                    {
                                        var sendResult = zaloClient.sendTextMessageToUserId(item.ZaloId, item.Content);
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
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
    }
}
