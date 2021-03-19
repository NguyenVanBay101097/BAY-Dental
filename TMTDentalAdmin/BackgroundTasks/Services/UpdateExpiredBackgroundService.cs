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
            var tenants = await context.Tenants.ToListAsync();
            var tasks = tenants.Select(x => ProcessUpdate(x.Id));
            await Task.WhenAll(tasks);
        }

        private async Task ProcessUpdate(Guid tenantId)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            optionsBuilder.UseSqlServer(_connectionStrings.TenantConnection);
            var context = new TenantDbContext(optionsBuilder.Options);

            var now = DateTime.Now;
            var history = await context.TenantExtendHistories.Where(x => x.TenantId == tenantId && now >= x.StartDate &&
                (x.ExpirationDate != x.AppTenant.DateExpired || x.ActiveCompaniesNbr != x.AppTenant.ActiveCompaniesNbr))
                   .OrderByDescending(x => x.StartDate)
                   .FirstOrDefaultAsync();
           
            if (history != null)
            {
                var tenant = context.Tenants.Find(tenantId);
                var oldDateExpired = tenant.DateExpired;
                var oldActiveCompaniesNbr = tenant.ActiveCompaniesNbr;
                tenant.ActiveCompaniesNbr = history.ActiveCompaniesNbr;
                tenant.DateExpired = history.ExpirationDate;
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
                }
                catch
                {
                    tenant.DateExpired = oldDateExpired;
                    tenant.ActiveCompaniesNbr = oldActiveCompaniesNbr;
                    context.SaveChanges();
                }
            }
        }
    }
}
