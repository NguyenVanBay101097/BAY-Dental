using ApplicationCore.Entities;
using Dapper;
using Hangfire;
using Infrastructure.Data;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Infrastructure.Services
{
    public class TCareJobService: ITCareJobService
    {
        private readonly ConnectionStrings _connectionStrings;

        public TCareJobService(IOptions<ConnectionStrings> connectionStrings)
        {
            _connectionStrings = connectionStrings?.Value;
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

                    var campaigns = conn.Query("SELECT * FROM TCareCampaigns").ToList();
                    foreach (var campaign in campaigns)
                    {
                        var messagings = conn.Query<TCareMessaging>("SELECT * FROM TCareMessagings WHERE TCareCampaignId = @id", new { id = campaign.Id }).ToList();
                        var messaging = messagings.FirstOrDefault();
                        if (messaging == null)
                            continue;

                        if (messaging.MethodType == "interval")
                        {
                            var date = DateTime.UtcNow;
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
                                date = date.AddDays(intervalNumber * 7);
                        } 
                        else if (messaging.MethodType == "schedule")
                        {
                            var date = messaging.SheduleDate.HasValue ? messaging.SheduleDate.Value.ToUniversalTime() : DateTime.UtcNow;
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
            foreach(var rule in rules)
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
                var date = today.AddDays(-beforeDays);

                var partner_ids = conn.Query<Guid>("SELECT Id FROM Partners WHERE Customer = 1 AND BirthDay = @day AND BirthMonth = @month", new { day = date.Day, month = date.Month }).ToList();
                return partner_ids;
            }

            return new List<Guid>();
        }
    }
}
