using ApplicationCore.Entities;
using Dapper;
using Infrastructure.Data;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
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
        private readonly ConnectionStrings _connectionStrings;

        public SaleOrderLineDateProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory, IOptions<ConnectionStrings> connectionStrings)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _connectionStrings = connectionStrings?.Value;
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

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.CatalogConnection);
                builder["Database"] = $"TMTDentalCatalogDb__{tenant.Hostname}";

                using (var conn = new SqlConnection(builder.ConnectionString))
                {
                    try
                    {
                        conn.Open();
                  
                        var lines = conn.Query<SaleOrderLine>("select * from SaleOrderLines where Date is null");
                        var orderIds = lines.Select(x => x.OrderId).Distinct().ToList();
                        var orders = conn.Query<SaleOrder>("select * from SaleOrders where id in @ids", new { ids = orderIds.ToArray() });
                        var orderDict = orders.ToDictionary(x => x.Id, x => x);

                        var affectedRows = conn.Execute("UPDATE SaleOrderLines SET Date = @Date WHERE Id = @Id;",
                          lines.Select(x => new {
                              Id = x.Id,
                              Date = orderDict[x.OrderId].DateOrder
                          }).ToArray());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
