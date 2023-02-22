using ApplicationCore.Entities;
using Dapper;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.TenantData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TMTDentalAdmin.BackgroundTasks.Services
{
    public class UpdateExpiredBackgroundService : BackgroundService
    {
        private readonly ILogger<UpdateExpiredBackgroundService> _logger;
        private readonly AdminAppSettings _appSettings;
        private readonly ConnectionStrings _connectionStrings;

        public UpdateExpiredBackgroundService(ILogger<UpdateExpiredBackgroundService> logger, IOptions<AdminAppSettings> appSettings,
            IOptions<ConnectionStrings> connectionStrings)
        {
            _logger = logger;
            _appSettings = appSettings?.Value;
            _connectionStrings = connectionStrings?.Value;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("UpdateExpiredBackgroundService is starting.");

            stoppingToken.Register(() => _logger.LogInformation("#1 UpdateExpiredBackgroundService background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("UpdateExpiredBackgroundService background task is doing background work.");

                try
                {
                    await CheckUpdateExpiredTenants();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }

                await Task.Delay(60000, stoppingToken);
            }

            _logger.LogInformation("UpdateExpiredBackgroundService background task is stopping.");
        }

        private async Task CheckUpdateExpiredTenants()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            optionsBuilder.UseSqlServer(_connectionStrings.TenantConnection);
            var context = new TenantDbContext(optionsBuilder.Options);

            var now = DateTime.Now;
            var histories = await context.TenantExtendHistories.Where(x => now >= x.StartDate && !x.ApplyDate.HasValue)
                .OrderBy(x => x.StartDate).ThenBy(x => x.DateCreated).ToListAsync();
            var tenantIds = histories.GroupBy(x => x.TenantId).Select(x => x.Key).ToList();
            var tenants = context.Tenants.Where(x => tenantIds.Contains(x.Id)).ToList();
            var tenantDict = tenants.ToDictionary(x => x.Id, x => x);
            foreach(var item in histories)
            {
                var tenant = tenantDict[item.TenantId];
                tenant.ActiveCompaniesNbr = item.ActiveCompaniesNbr;
                tenant.DateExpired = item.ExpirationDate;
                item.ApplyDate = DateTime.Now;
            }

            context.SaveChanges();

            var allTasks = tenants.Select(x => CallApiClearCache(x));

            try
            {
                await Task.WhenAll(allTasks.ToArray());
            }
            catch
            {
                _logger.LogError("UpdateExpiredBackgroundService clear cache fail");
            }
        }

        private async Task CallApiClearCache(AppTenant tenant)
        {
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => { return true; };

                HttpResponseMessage response = null;
                using (var client = new HttpClient(new RetryHandler(clientHandler)))
                {
                    response = await client.GetAsync($"{_appSettings.Schema}://{tenant.Hostname}.{_appSettings.CatalogDomain}/api/Companies/ClearCacheTenant");
                }


                _logger.LogInformation("UpdateExpiredBackgroundService success " + response.StatusCode);
            }
            catch(Exception e)
            {
                _logger.LogInformation("UpdateExpiredBackgroundService fail " + e.Message);
            }
        }
    }
}
