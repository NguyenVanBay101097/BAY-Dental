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
                    var activity = conn.Query<MarketingCampaignActivityQuery>("" +
                        "SELECT a.Condition, a.ActivityType, a.Content, c.DateStart as CampaignDateStart, a.AutoTakeCoupon, a.CouponProgramId " +
                        "FROM MarketingCampaignActivities a " +
                        "LEFT JOIN MarketingCampaigns c on a.CampaignId = c.Id " +
                        "where a.Id = @id" +
                        "", new { id = activityId }).FirstOrDefault();

                    if (activity != null)
                    {
                        var list = new List<PartnerSendMessageResult>();
                        if (activity.Condition == "no_sales")
                        {
                            var dateFrom = activity.CampaignDateStart ?? DateTime.Now;
                            var dateTo = DateTime.Now;
                            var result = conn.Query<Partner>("" +
                                "SELECT * " +
                                "FROM Partners p " +
                                "where p.Customer = 1 and p.Id not in ( " +
                                "SELECT s.PartnerId " +
                                "FROM SaleOrders s " +
                                "WHERE s.DateOrder >= @dateFrom and s.DateOrder <= @dateTo " +
                                "group by s.PartnerId)" +
                                "", new { dateFrom, dateTo }).ToList();

                            list = result.Select(x => new PartnerSendMessageResult
                            {
                                Content = activity.Content,
                                ZaloId = x.ZaloId
                            }).ToList();
                        }
                      
                        if (list.Any())
                        {
                            if (activity.ActivityType == "zalo")
                            {
                                var zaloConfig = conn.Query<ZaloOAConfig>("SELECT * FROM ZaloOAConfigs").FirstOrDefault();
                                if (zaloConfig != null && !string.IsNullOrEmpty(activity.Content))
                                {
                                    var zaloClient = new ZaloClient(zaloConfig.AccessToken);
                                    foreach (var item in list)
                                    {
                                        if (string.IsNullOrEmpty(item.ZaloId))
                                            continue;
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

        public class MarketingCampaignActivityQuery
        {
            public string Condition { get; set; }

            public string Content { get; set; }

            public string ActivityType { get; set; }

            public DateTime? CampaignDateStart { get; set; }

            public bool? AutoTakeCoupon { get; set; }

            public Guid? CouponProgramId { get; set; }
        }
    }
}
