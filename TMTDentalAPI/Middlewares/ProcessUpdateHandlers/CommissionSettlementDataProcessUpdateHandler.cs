using ApplicationCore.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TMTDentalAPI.Middlewares.ProcessUpdateHandlers
{
    public class CommissionSettlementDataProcessUpdateHandler : INotificationHandler<ProcessUpdateNotification>
    {
        private const string _version = "1.0.2.1";
        private IServiceScopeFactory _serviceScopeFactory;


        public CommissionSettlementDataProcessUpdateHandler(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;

        }

        public Task Handle(ProcessUpdateNotification notification, CancellationToken cancellationToken)
        {
            //nếu version tenant mà nhỏ hơn version app setting
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var tenant = scope.ServiceProvider.GetService<AppTenant>();
                if (tenant != null)
                {
                    //Version version1 = new Version(_version);
                    //Version version2 = new Version(tenant.Version);
                    //if (version2.CompareTo(version1) >= 0)
                    //    return Task.CompletedTask;
                }

                var scopedServices = scope.ServiceProvider;
                var context = scope.ServiceProvider.GetService<CatalogDbContext>();

                var commissionSettlements = context.CommissionSettlements.Where(x => !x.PartnerId.HasValue || !x.CompanyId.HasValue)
                    .Include(x => x.Employee)
                    .Include(x => x.Agent)
                    .Include(x => x.SaleOrderLine)
                    .ToList();

                foreach(var settlement in commissionSettlements)
                {
                    if (!settlement.PartnerId.HasValue)
                        settlement.PartnerId = settlement.Employee != null ? settlement.Employee.PartnerId : (settlement.Agent != null ? settlement.Agent.PartnerId : null);
                    if (!settlement.CompanyId.HasValue)
                        settlement.CompanyId = settlement.SaleOrderLine != null ? settlement.SaleOrderLine.CompanyId : (settlement.MoveLine != null ? settlement.MoveLine.CompanyId : null);
                }

                context.SaveChanges();
            }

            return Task.CompletedTask;
        }
    }
}
