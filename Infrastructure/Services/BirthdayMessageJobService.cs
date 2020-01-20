using ApplicationCore.Entities;
using Dapper;
using Infrastructure.Data;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ZaloDotNetSDK;

namespace Infrastructure.Services
{
    public class BirthdayMessageJobService
    {
        private readonly ConnectionStrings _connectionStrings;
        public BirthdayMessageJobService(IOptions<ConnectionStrings> connectionStrings)
        {
            _connectionStrings = connectionStrings?.Value;
        }
        public void SendMessage(string db)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.CatalogConnection);
            builder["Database"] = $"TMTDentalCatalogDb__{db}";
            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    conn.Open();
                    var zaloConfig = conn.Query<ZaloOAConfig>("SELECT * FROM ZaloOAConfigs").FirstOrDefault();
                    if (zaloConfig != null && !string.IsNullOrEmpty(zaloConfig.BirthdayMessageContent))
                    {
                        var today = DateTime.Today;
                        var userIds = conn.Query<string>(
                            @"SELECT ZaloId FROM Partners WHERE ZaloId IS NOT NULL AND Customer = 1 AND BirthMonth = @month AND BirthDay = @day",
                            new { month = today.Month, day = today.Day });
                        var zaloClient = new ZaloClient(zaloConfig.AccessToken);
                        foreach (var userId in userIds)
                        {
                            var sendResult = zaloClient.sendTextMessageToUserId(userId, zaloConfig.BirthdayMessageContent);
                        }
                    }
                }
                catch
                {
                }
            }
        }
    }
}
