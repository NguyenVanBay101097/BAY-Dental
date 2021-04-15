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
            _logger.LogDebug("UpdateExpiredBackgroundService is starting.");

            stoppingToken.Register(() => _logger.LogDebug("#1 UpdateExpiredBackgroundService background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("UpdateExpiredBackgroundService background task is doing background work.");

                await CheckUpdateExpiredTenants();

                await Task.Delay(60000, stoppingToken);
            }

            _logger.LogDebug("UpdateExpiredBackgroundService background task is stopping.");
        }

        private async Task CheckUpdateExpiredTenants()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            optionsBuilder.UseSqlServer(_connectionStrings.TenantConnection);
            var context = new TenantDbContext(optionsBuilder.Options);

            var now = DateTime.Now;
            var histories = await context.TenantExtendHistories.Where(x => now >= x.StartDate && !x.ApplyDate.HasValue)
                .OrderBy(x => x.StartDate).ThenBy(x => x.DateCreated).ToListAsync();
            var dict = histories.GroupBy(x => x.TenantId).ToDictionary(x => x.Key, x => x.OrderBy(s => s.StartDate).ThenBy(x => x.DateCreated).First());
            var throttler = new SemaphoreSlim(10);
            var allTasks = new List<Task>();
            foreach (var item in dict)
            {
                allTasks.Add(Task.Run(async () =>
                {
                    await throttler.WaitAsync();
                    try
                    {
                        await ProcessUpdate(item.Key, item.Value, context);
                    }
                    finally
                    {
                        throttler.Release();
                    }
                }));
            }
        

            Task.WaitAll(allTasks.ToArray()); //block main thread wail all complete
        }

        private async Task ProcessUpdate(Guid tenantId, TenantExtendHistory history, TenantDbContext context)
        {
            var tenant = context.Tenants.Find(tenantId);
            var oldDateExpired = tenant.DateExpired;
            var oldActiveCompaniesNbr = tenant.ActiveCompaniesNbr;
            tenant.ActiveCompaniesNbr = history.ActiveCompaniesNbr;
            tenant.DateExpired = history.ExpirationDate;
            history.ApplyDate = DateTime.Now;
            context.SaveChanges();

            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => { return true; };

                HttpResponseMessage response = null;
                using (var client = new HttpClient(new RetryHandler(clientHandler)))
                {
                    response = await client.GetAsync($"{_appSettings.Schema}://{tenant.Hostname}.{_appSettings.CatalogDomain}/api/Companies/ClearCacheTenant");
                }

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Có lỗi xảy ra");

                _logger.LogInformation("UpdateExpiredBackgroundService success");
            }
            catch(Exception e)
            {
                _logger.LogInformation("UpdateExpiredBackgroundService fail " + e.Message);
                tenant.DateExpired = oldDateExpired;
                tenant.ActiveCompaniesNbr = oldActiveCompaniesNbr;
                history.ApplyDate = null;
                context.SaveChanges();
            }
        }
    }
}
