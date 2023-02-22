using ApplicationCore.Entities;
using Dapper;
using Infrastructure.Data;
using Infrastructure.TenantData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ScheduleJobService : CronJobService
    {
        private readonly ILogger<ScheduleJobService> _logger;
        private readonly string _connection;
        private readonly AdminAppSettings _appSettings;
        private UpdateExpiredDateTenantService _updateExpiredDateTenantService;

        public ScheduleJobService(IScheduleConfig<ScheduleJobService> config, ILogger<ScheduleJobService> logger)
            : base(config.ConnectionStrings, config.appSettings, config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _connection = config.ConnectionStrings;
            _appSettings = config.appSettings;
            _updateExpiredDateTenantService = new UpdateExpiredDateTenantService(_appSettings);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ScheduleJob starts.");
            return base.StartAsync(cancellationToken);
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} ScheduleJob is working.");
            await CheckUpdateDateExpiredAuto();
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ScheduleJob is stopping.");
            return base.StopAsync(cancellationToken);
        }

        public async Task CheckUpdateDateExpiredAuto()
        {
            var today = DateTime.Today;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connection);
            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    conn.Open();
                    var tenants = await conn.QueryAsync<AppTenant>("SELECT * FROM Tenants WHERE DateExpired <= @today", new { today = today });
                    await _updateExpiredDateTenantService.ComputeTenant(tenants, conn);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}