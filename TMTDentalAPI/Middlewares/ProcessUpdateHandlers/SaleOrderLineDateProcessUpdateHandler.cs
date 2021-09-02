using ApplicationCore.Entities;
using Dapper;
using Infrastructure.Data;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TMTDentalAPI.Middlewares.ProcessUpdateHandlers
{
    public class SaleOrderLineDateProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.1.9";
        private IServiceScopeFactory _serviceScopeFactory;
        public SaleOrderLineDateProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task Handle(ProcessUpdateNotification notification, CancellationToken cancellationToken)
        {
            //nếu version tenant mà nhỏ hơn version app setting
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var tenant = scope.ServiceProvider.GetService<AppTenant>();
                if (tenant == null)
                    return Task.CompletedTask;

                Version version1 = new Version(_version);
                Version version2 = new Version(tenant.Version);
                if (version2.CompareTo(version1) >= 0)
                    return Task.CompletedTask;

                var scopedServices = scope.ServiceProvider;
                var context = scope.ServiceProvider.GetService<CatalogDbContext>();
                var connectionStrings = scope.ServiceProvider.GetService<ConnectionStrings>();
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionStrings.CatalogConnection);
                builder["Database"] = $"TMTDentalCatalogDb__{tenant.Hostname}";

                using (var conn = new SqlConnection(builder.ConnectionString))
                {
                    try
                    {
                        conn.Open();

                       
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }


                var orderLines = context.SaleOrderLines.Where(x => !x.Date.HasValue)
                    .ToList();
                foreach (var line in orderLines)
                {
                    line.Date = line.DateCreated;
                }

                context.SaveChanges();
              
            }

            return Task.CompletedTask;
        }
    }
}
